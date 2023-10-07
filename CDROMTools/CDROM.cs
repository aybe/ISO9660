using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CDROMTools.Interop;
using CDROMTools.Iso9660;
using CDROMTools.Utils;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace CDROMTools
{
    /// <summary>
    ///     Main class to work against a CD-ROM: extract sectors, tracks or files.
    /// </summary>
    public sealed class CDROM : IDisposable
    {
        #region Constructor

        /// <summary>
        ///     Create a new instance of <see cref="CDROM" /> (see Remarks).
        /// </summary>
        /// <param name="info">
        ///     An instance representing drive information for a CD-ROM, use <see cref="GetCDROMDrives" /> to get a list of valid
        ///     drives.
        /// </param>
        /// <remarks>
        ///     Constructor opens the drive, reads the TOC, reads the descriptors and if any, reads the files. Consequently,
        ///     it can take a few seconds to open a CD-ROM that has an ISO9660 file system with many files in it.
        /// </remarks>
        public CDROM(DriveInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (info.DriveType != DriveType.CDRom)
                throw new ArgumentOutOfRangeException(nameof(info), "Not a CD-ROM drive");

            // open drive
            _handle = OpenDrive(info);
            if (_handle.IsInvalid) throw new Win32Exception("Unable to open CD-ROM drive");

            // get tracks/bounds
            Tracks = GetTracks(_handle);
            if (!Tracks.Any()) throw new InvalidOperationException("No tracks found, is media in ?");
            GetTracksBounds(Tracks, out _lbaMin, out _lbaMax);

            // get descriptors
            var descriptors = ReadDescriptors();
            if (descriptors != null)
                PrimaryVolumeDescriptor = descriptors.OfType<PrimaryVolumeDescriptor>().FirstOrDefault();

            // get files
            if (PrimaryVolumeDescriptor != null) Files = ReadFiles();
        }

        #endregion

        #region Constants

        private const uint UserDataSize = 2048u;
        /// <summary>
        /// Size in bytes of a 'RAW' sector.
        /// </summary>
        public const uint RawSectorSize = 2352u;

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        ///     Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage
        ///     collection.
        /// </summary>
        ~CDROM()
        {
            Dispose(false);
        }

        #endregion

        #region Fields

        private readonly SafeFileHandle _handle;
        private readonly uint _lbaMax;
        private readonly uint _lbaMin;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the list of files for this instance (if media has an ISO9660 file system).
        /// </summary>
        public CDROMFile[] Files { get; }

        /// <summary>
        ///     Gets the primary volume descriptor for this instance (if media has an ISO9660 file system).
        /// </summary>
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; }

        /// <summary>
        ///     Gets the list of tracks for this instance.
        /// </summary>
        public CDROMTrack[] Tracks { get; }

        #endregion

        #region Methods (instance)

        /// <summary>
        ///     Gets the mode a file has been recorded in (see Remarks).
        /// </summary>
        /// <param name="file">File to get mode of.</param>
        /// <param name="progress">Provider for progress updates, can be <c>null</c>.</param>
        /// <param name="fastDetection">
        ///     Whether to perform a fast detection, in most cases it will be sufficient. Set to
        ///     <c>false</c> if you're experiencing wrong results.
        /// </param>
        /// <param name="detectionPercentage">
        ///     If <paramref name="fastDetection" /> is <c>true</c>, defines the percentage that is scanned
        ///     for <paramref name="file" />, from 1 to 100%.
        /// </param>
        /// <returns>
        ///     Detected file mode. If <see cref="CDROMFileMode.Mode2Mixed" /> is returned, file should be extracted in 'RAW
        ///     mode' (<see cref="ReadFileRaw" />) as it's likely to be unreadable otherwise. For all other modes, file should be
        ///     extracted in 'user mode' (<see cref="ReadFileUser" />).
        /// </returns>
        /// <remarks>
        ///     Data files are normally recorded either in <see cref="CDROMFileMode.Mode1" /> or
        ///     <see cref="CDROMFileMode.Mode2Form1" /> modes as these types of sectors provide the maximum amount of
        ///     error-correcting code (ECC/EDC) for 'user data'. On the other hand, sectors of mode <see cref="CDROMFileMode.Mode2Form2"/> and <see cref="CDROMFileMode.Mode2Mixed"/> are normally used for multimedia content where reading errors are considered 'acceptable'.
        /// </remarks>
        public CDROMFileMode GetFileMode([NotNull] CDROMFile file, [CanBeNull] IProgress<double> progress = null,
            bool fastDetection = true, int detectionPercentage = 10)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            var blocks = GetFileBlocks(file);
            var helper = progress != null ? new ProgressHelper(progress, (int) blocks) : null;
            var percentage1 = Clamp(detectionPercentage, 1, 100);
            var iterations = // accounts for 1 sector long files
                (uint) Math.Max(1, blocks/100.0d*percentage1);
            var set = new HashSet<CDROMSectorMode>();
            var lba = (uint) file.Lba;
            for (var i = 0u; i < blocks; i++)
            {
                if (fastDetection && (set.Count > 1 || i >= iterations))
                    break; // handle mixed/non-mixed content
                var lba1 = lba + i;
                var raw = ReadSectorRaw(lba1);
                var mode = GetSectorMode(raw);
                set.Add(mode);
                helper?.Update((int) i);
            }
            var fileMode = GetFileMode(set);
            return fileMode;
        }


        /// <summary>
        ///     Gets if a sector is of type 'audio'.
        /// </summary>
        /// <param name="lba">Sector to check.</param>
        /// <returns><c>true</c> if sector is of type 'audio'.</returns>
        public bool IsAudioSector(uint lba)
        {
            ThrowIfInvalidLba(lba);
            var track = Tracks.Single(s => lba >= s.LBA && lba < s.LBA + s.LBACount);
            var isAudioSector = track.TrackType == CDROMTrackType.Audio;
            return isAudioSector;
        }

        /// <summary>
        ///     Reads a file in 'RAW mode'.
        /// </summary>
        /// <param name="file">File to read.</param>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="progress">Provider for progress updates, can be <c>null</c>.</param>
        public void ReadFileRaw(CDROMFile file, Stream stream, IProgress<double> progress)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            ThrowIfStreamNotWriteable(stream);
            var blocks = GetFileBlocks(file);
            var helper = progress != null ? new ProgressHelper(progress, (int) blocks) : null;
            for (var i = 0u; i < blocks; i++)
            {
                var lba = file.Lba + i;
                var sector = ReadSectorRaw(lba);
                stream.Write(sector, 0, sector.Length);
                helper?.Update((int) i);
            }
        }

        /// <summary>
        ///     Reads a file in 'user mode'.
        /// </summary>
        /// <param name="file">File to read.</param>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="progress">Provider for progress updates, can be <c>null</c>.</param>
        public void ReadFileUser(CDROMFile file, Stream stream, IProgress<double> progress)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            ThrowIfStreamNotWriteable(stream);
            var blocks = GetFileBlocks(file);
            var helper = progress != null ? new ProgressHelper(progress, (int) blocks) : null;
            var lba = (uint) file.Lba;
            var remaining = (long) file.Size;
            for (var i = 0u; i < blocks; i++)
            {
                var lba1 = lba + i;
                var sector = ReadSectorUser(lba1);
                var data = sector.GetUserData();
                var count = Math.Min(remaining, data.Length);
                stream.Write(data, 0, (int) count);
                remaining -= data.Length;
                helper?.Update((int) i);
            }
        }


        /// <summary>
        ///     Reads a sector in 'RAW mode'.
        /// </summary>
        /// <param name="lba">Address of the sector to read.</param>
        /// <returns>Read sector as an array of bytes.</returns>
        public byte[] ReadSectorRaw(uint lba)
        {
            ThrowIfInvalidLba(lba);
            const uint length = RawSectorSize;
            var buffer = new byte[length];
            using (var pBuffer = new PinnedScope(buffer))
            {
                // send a command that reads ALL from ANY type of sector
                var sptd = new ScsiPassThroughDirect(12);
                var sptdSize = (ushort) Marshal.SizeOf<ScsiPassThroughDirect>();
                sptd.Length = sptdSize;
                sptd.DataIn = NativeConstants.SCSI_IOCTL_DATA_IN;
                sptd.DataTransferLength = length;
                sptd.DataBuffer = pBuffer.Address;
                sptd.TimeOutValue = 60;
                sptd.Cdb[0] = 0xBE;
                sptd.Cdb[2] = BitUtils.HIBYTE(BitUtils.HIWORD(lba));
                sptd.Cdb[3] = BitUtils.LOBYTE(BitUtils.HIWORD(lba));
                sptd.Cdb[4] = BitUtils.HIBYTE(BitUtils.LOWORD(lba));
                sptd.Cdb[5] = BitUtils.LOBYTE(BitUtils.LOWORD(lba));
                sptd.Cdb[6] = BitUtils.LOBYTE(BitUtils.HIWORD(1));
                sptd.Cdb[7] = BitUtils.HIBYTE(BitUtils.LOWORD(1));
                sptd.Cdb[8] = BitUtils.LOBYTE(BitUtils.LOWORD(1));
                sptd.Cdb[9] = 0xF8;
                var pSptd = Marshal.AllocHGlobal(sptdSize);
                Marshal.StructureToPtr(sptd, pSptd, true);
                IOCTL(_handle, NativeConstants.IOCTL_SCSI_PASS_THROUGH_DIRECT, pSptd, sptdSize, pSptd, 0u);
                var sptd1 = Marshal.PtrToStructure<ScsiPassThroughDirect>(pSptd);
                if (sptd1.DataTransferLength != length) // ever happens ?!
                    throw new InvalidOperationException("Not enough data received");
                Marshal.FreeHGlobal(pSptd);
            }
            return buffer;
        }

        /// <summary>
        ///     Reads a sector in 'user mode'.
        /// </summary>
        /// <param name="lba">Address of the sector to read.</param>
        /// <returns>Read sector as a typed sector.</returns>
        public ISector ReadSectorUser(uint lba)
        {
            var raw = ReadSectorRaw(lba);
            var isAudio = IsAudioSector(lba);
            var mode = isAudio ? CDROMSectorMode.Audio : GetSectorMode(raw); // see docs
            switch (mode)
            {
                case CDROMSectorMode.Mode0:
                    return SectorUtils.FromBytes<SectorMode0>(raw);
                case CDROMSectorMode.Mode1:
                    return SectorUtils.FromBytes<SectorMode1>(raw);
                case CDROMSectorMode.Mode2Formless:
                    return SectorUtils.FromBytes<SectorMode2Formless>(raw);
                case CDROMSectorMode.Mode2Form1:
                    return SectorUtils.FromBytes<SectorMode2Form1>(raw);
                case CDROMSectorMode.Mode2Form2:
                    return SectorUtils.FromBytes<SectorMode2Form2>(raw);
                case CDROMSectorMode.Reserved:
                    return SectorUtils.FromBytes<SectorReserved>(raw);
                case CDROMSectorMode.Audio:
                    return SectorUtils.FromBytes<SectorAudio>(raw);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Reads a track in 'RAW mode'.
        /// </summary>
        /// <param name="track">Track to read.</param>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="progress">Provider for progress updates, can be <c>null</c>.</param>
        public void ReadTrackRaw(CDROMTrack track, Stream stream, IProgress<double> progress)
        {
            if (track == null) throw new ArgumentNullException(nameof(track));
            ThrowIfStreamNotWriteable(stream);
            var lba = track.LBA;
            var count = track.LBACount;
            ProgressHelper helper = null;
            if (progress != null) helper = new ProgressHelper(progress, (int) count);
            for (var i = 0u; i < count; i++)
            {
                var lba1 = lba + i;
                var raw = ReadSectorRaw(lba1);
                stream.Write(raw, 0, raw.Length);
                helper?.Update((int) i);
            }
        }

        private VolumeDescriptor[] ReadDescriptors()
        {
            var tracks = Tracks; // never null
            var track = tracks.FirstOrDefault();
            if (track == null || track.TrackType != CDROMTrackType.Data) return null;

            var list = new List<VolumeDescriptor>();
            var lba = 16u;
            while (true)
            {
                var sector = ReadSectorUser(lba);
                using (var reader = sector.GetUserDataReader())
                {
                    var item = VolumeDescriptor.TryCreate(reader);
                    if (item == null) break;
                    list.Add(item);
                    if (item is VolumeDescriptorSetTerminator) break;
                    // NOTE no point in reading all the terminators, if any
                }

                if (list.Count > 5) // max allowed by standard
                    throw new InvalidOperationException("Too many descriptors encountered.");

                lba++;
            }
            return list.ToArray();
        }

        private CDROMFile[] ReadFiles()
        {
            var pvd = PrimaryVolumeDescriptor; // never null
            var sector = ReadSectorUser(pvd.LocationTypeLPathTable);
            using (var reader = sector.GetUserDataReader())
            {
                // get path table (all directories)
                var keys = new List<PathTableRecord>();
                while (reader.BaseStream.Position < pvd.PathTableSize)
                {
                    var record = new PathTableRecord(reader);
                    keys.Add(record);
                }

                // get each files in a path table record
                var dictionary = new Dictionary<PathTableRecord, DirectoryRecord[]>();
                foreach (var key in keys)
                {
                    var records = GetTableRecords(key);
                    var value = records.Where(s => s.FileFlags == FileFlags.File).ToArray();
                    dictionary.Add(key, value);
                }

                // build final form of files
                var files = GetFileList(dictionary);
                return files;
            }
        }

        private void ThrowIfInvalidLba(uint lba)
        {
            if (lba < _lbaMin || lba > _lbaMax)
                throw new ArgumentOutOfRangeException(nameof(lba), "Invalid LBA");
        }

        #endregion

        #region Methods (static)

        /// <summary>
        ///     Gets the drives in the system that are of type <see cref="DriveType.CDRom" />.
        /// </summary>
        /// <returns>List of CDROM drives, note that a drive can be accessible even though no media is inserted.</returns>
        public static DriveInfo[] GetCDROMDrives()
        {
            var drives = DriveInfo.GetDrives();
            var @where = drives.Where(s => s.DriveType == DriveType.CDRom).ToArray();
            return @where;
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        ///     Gets the length in blocks for a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static uint GetFileBlocks(CDROMFile file)
        {
            var max = Math.Max(file.Size, UserDataSize);
            var count = (double) max/UserDataSize;
            var blocks = (uint) Math.Ceiling(count);
            return blocks;
        }

        /// <summary>
        ///     Gets the list of files in a map of 'path table records => directory records'.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static CDROMFile[] GetFileList(Dictionary<PathTableRecord, DirectoryRecord[]> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            var list = new List<CDROMFile>();
            foreach (var pair in dictionary)
            {
                var key = pair.Key;
                var value = pair.Value;
                foreach (var record in value)
                {
                    var fullPath = GetFilePath(key, record, dictionary);
                    var lba = record.ExtentLocation;
                    var size = record.DataLength;

                    var file = new CDROMFile(fullPath, lba, size);
                    list.Add(file);
                }
            }
            return list.ToArray();
        }

        private static CDROMFileMode GetFileMode([NotNull] HashSet<CDROMSectorMode> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            if (set.Count == 0) throw new ArgumentException("Argument is empty collection", nameof(set));

            var modes = new HashSet<CDROMSectorMode>(set);

            var hasM1 = modes.Contains(CDROMSectorMode.Mode1);
            if (hasM1) modes.Remove(CDROMSectorMode.Mode1);

            var hasM2F1 = modes.Contains(CDROMSectorMode.Mode2Form1);
            if (hasM2F1) modes.Remove(CDROMSectorMode.Mode2Form1);

            var hasM2F2 = modes.Contains(CDROMSectorMode.Mode2Form2);
            if (hasM2F2) modes.Remove(CDROMSectorMode.Mode2Form2);

            var any = modes.Any();
            if (!any)
            {
                if (hasM2F1 && hasM2F2) return CDROMFileMode.Mode2Mixed;
                if (hasM2F1) return CDROMFileMode.Mode2Form1;
                if (hasM2F2) return CDROMFileMode.Mode2Form2;
                if (hasM1) return CDROMFileMode.Mode1;
            }

            var format = $"Invalid mix of sectors modes: {string.Join(", ", set)}";
            throw new InvalidOperationException(format); // should never happen though
        }

        /// <summary>
        ///     Gets the full path to a file for a directory record that is a file.
        /// </summary>
        /// <param name="pathTableRecord"></param>
        /// <param name="directoryRecord"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static string GetFilePath(PathTableRecord pathTableRecord, DirectoryRecord directoryRecord,
            Dictionary<PathTableRecord, DirectoryRecord[]> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            // NOTE unless in GetFilesInDirectoryRecords(DirectoryRecord[]) we can directly check against 'Directory'
            if (directoryRecord.FileFlags.HasFlag(FileFlags.Directory))
            {
                throw new ArgumentOutOfRangeException(nameof(directoryRecord),
                    @"Expecting a directory record representing a file.");
            }

            // get file name, strip-out version
            var fileName = Regex.Match(directoryRecord.FileIdentifier, @".*(?=;)").Value;

            // now build path from bottom to top, stopping at '\'
            var stack = new Stack<string>();
            stack.Push(fileName);
            var ptr = pathTableRecord;
            while (true)
            {
                var folder = ptr.DirectoryIdentifier;
                if (folder == @"\") break;
                stack.Push(folder);
                var index = ptr.ParentDirectoryNumber - 1;
                var ptr1 = dictionary.Keys.ElementAt(index);
                ptr = ptr1;
            }
            var path = $@"\{string.Join(@"\", stack)}"; // make it 'friendly'
            return path;
        }

        /// <summary>
        ///     Gets the mode of a sector (see Remarks).
        /// </summary>
        /// <param name="bytes">Sector as an array of bytes.</param>
        /// <returns>Sector mode found.</returns>
        /// <remarks>
        ///     <para>
        ///         For Mode 2 sectors, order of precedence is: <see cref="CDROMSectorMode.Mode2Form1" /> (if EDC matches),
        ///         <see cref="CDROMSectorMode.Mode2Form2" /> (if sector has <see cref="SectorMode2Form1SubHeaderSubMode.Form2" />
        ///         flag), <see cref="CDROMSectorMode.Mode2Formless" />.
        ///     </para>
        ///     <para>
        ///         For audio sectors, you must use <see cref="IsAudioSector" /> instead since they don't have any header.
        ///     </para>
        /// </remarks>
        private static CDROMSectorMode GetSectorMode(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length < RawSectorSize) throw new ArgumentOutOfRangeException(nameof(bytes));
            var sector = SectorUtils.FromBytes<Sector>(bytes);
            switch (sector.Header.Mode)
            {
                case SectorMode.Mode0:
                    return CDROMSectorMode.Mode0;
                case SectorMode.Mode1:
                    return CDROMSectorMode.Mode1;
                case SectorMode.Mode2:
                    var m2F1 = SectorUtils.FromBytes<SectorMode2Form1>(bytes);
                    //var data = bytes.Skip(16).Take(2056).ToArray();
                    //var edc1 = EdcHelper.ComputeBlock(0, data);
                    var edc1 = EdcHelper.ComputeBlock(0, bytes, 16, 2056);
                    var edc2 = BitConverter.ToUInt32(m2F1.Edc, 0);
                    var isM2F1 = edc1 == edc2;
                    if (isM2F1) return CDROMSectorMode.Mode2Form1;

                    // NOTE we cannot reliably check EDC of M2F2 since it's optional
                    var isForm2 =
                        m2F1.SubHeaderCopy1.SubMode.HasFlag(SectorMode2Form1SubHeaderSubMode.Form2) &&
                        m2F1.SubHeaderCopy2.SubMode.HasFlag(SectorMode2Form1SubHeaderSubMode.Form2);
                    if (isForm2) return CDROMSectorMode.Mode2Form2;

                    return CDROMSectorMode.Mode2Formless;
                case SectorMode.Reserved:
                    return CDROMSectorMode.Reserved;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Gets the directory records for a path table record.
        /// </summary>
        /// <param name="pathTableRecord"></param>
        /// <returns></returns>
        private DirectoryRecord[] GetTableRecords(PathTableRecord pathTableRecord)
        {
            // get the first entry "." which contains the length in bytes to read
            uint length;
            var sector = ReadSectorUser(pathTableRecord.ExtentLocation);
            using (var reader = sector.GetUserDataReader())
            {
                var record = new DirectoryRecord(reader);
                length = record.DataLength.Value;
            }

            // now grab all the records
            var records = new List<DirectoryRecord>();
            var sectors = (int) Math.Ceiling((double) length/UserDataSize);
            for (var i = 0u; i < sectors; i++)
            {
                var lba = pathTableRecord.ExtentLocation + i;
                var sector1 = ReadSectorUser(lba);
                using (var reader = sector1.GetUserDataReader())
                {
                    while (true)
                    {
                        var record = new DirectoryRecord(reader);
                        if (record.DirectoryRecordLength <= 0)
                        {
                            // early exit as 'record' will not try further parsing when length is 0
                            break;
                        }
                        records.Add(record);
                    }
                }
            }
            return records.ToArray();
        }

        /// <summary>
        ///     Gets tracks for a CD-ROM handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>Array of tracks, will be of length 0 if there aren't any (empty drive).</returns>
        private static CDROMTrack[] GetTracks(SafeFileHandle handle)
        {
            if (handle == null) throw new ArgumentNullException(nameof(handle));

            // get TOC : allocate/grab/marshal back
            var size = Marshal.SizeOf<CdRomToc>();
            var buffer = Marshal.AllocHGlobal(size);
            NativeMethods.RtlZeroMemory(buffer, (uint) size);
            IOCTL(handle, NativeConstants.IOCTL_CDROM_READ_TOC, IntPtr.Zero, 0, buffer, (uint) size);
            var toc = Marshal.PtrToStructure<CdRomToc>(buffer);
            Marshal.FreeHGlobal(buffer);

            // build track-listing from TOC
            var list = new List<CDROMTrack>();
            if (toc.Length > 0) // a drive can be opened even though no CD is in
            {
                for (var i = toc.FirstTrack - 1; i < toc.LastTrack; i++)
                {
                    var data0 = toc.TrackData[i];
                    var data1 = toc.TrackData[i + 1];
                    var lba0 = MSF2LBA(data0.Address);
                    var lba1 = MSF2LBA(data1.Address);
                    var trackType = data0.Control == SubChannelQControlBits.AudioDataTrack
                        ? CDROMTrackType.Data
                        : CDROMTrackType.Audio;
                    var track = new CDROMTrack(i + 1, lba0, lba1 - lba0, trackType);
                    list.Add(track);
                }
            }
            return list.ToArray();
        }

        private static void GetTracksBounds([NotNull] CDROMTrack[] tracks, out uint lbaMin, out uint lbaMax)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));
            if (tracks.Length == 0) throw new ArgumentException("Argument is empty collection", nameof(tracks));

            var first = tracks.First();
            var last = tracks.Last();
            lbaMin = first.LBA;
            lbaMax = last.LBA + last.LBACount - 1;
        }

        /// <summary>
        ///     Executes an IOCTL code.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ioControlCode"></param>
        /// <param name="inputBuffer">Can be <see cref="IntPtr.Zero" />.</param>
        /// <param name="inputBufferSize"></param>
        /// <param name="outputBuffer">Can be <see cref="IntPtr.Zero" />.</param>
        /// <param name="outputBufferSize"></param>
        /// <returns>
        ///     Amount of bytes returned by DeviceIoControl if call was not overlapped, otherwise amount of bytes transferred
        ///     by GetOverlappedResult.
        /// </returns>
        /// <exception cref="InvalidOperationException">When an irrecoverable error has occurred.</exception>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static uint IOCTL(SafeFileHandle handle, uint ioControlCode,
            IntPtr inputBuffer, uint inputBufferSize, IntPtr outputBuffer, uint outputBufferSize)
        {
            if (handle == null) throw new ArgumentNullException(nameof(handle));

            // create new overlapped to wait for async I/O (if possible)
            var overlapped = new Overlapped {hEvent = NativeMethods.CreateEvent(IntPtr.Zero, true, false, null)};
            if (overlapped.hEvent == IntPtr.Zero)
            {
                var exception = new Win32Exception();
                throw new InvalidOperationException("Could not create event for overlapped operation.", exception);
            }

            // execute control code but do not throw if failed, see below
            uint returned;
            var ioctl = NativeMethods.DeviceIoControl(handle, ioControlCode,
                inputBuffer, inputBufferSize, outputBuffer, outputBufferSize, out returned, ref overlapped);
            if (ioctl) return returned;

            // wait for pending I/O, throw on failure (note: overlapped does not apply for SPTD calls)
            var exception1 = new Win32Exception();
            if (exception1.NativeErrorCode != NativeConstants.ERROR_IO_PENDING)
                return returned; // already finished
            var result = NativeMethods.GetOverlappedResult(handle, ref overlapped, out returned, true);
            if (!result) // throw now
            {
                var exception = new Win32Exception();
                throw new InvalidOperationException("Could not retrieve result for overlapped operation.", exception);
            }
            NativeMethods.CloseHandle(overlapped.hEvent);

            return returned;
        }

        private static uint MSF2LBA(byte[] msf)
        {
            if (msf == null) throw new ArgumentNullException(nameof(msf));
            var block = msf[1]*NativeConstants.CD_BLOCKS_PER_SECOND*60u +
                        msf[2]*NativeConstants.CD_BLOCKS_PER_SECOND +
                        msf[3];
            var lba = (uint) (block - 150u);
            return lba;
        }

        private static SafeFileHandle OpenDrive(DriveInfo info)
        {
            var path = info.RootDirectory.Name.Substring(0, 2);
            var handle = NativeMethods.CreateFileW($@"\\.\{path}", FileAccess.Read | FileAccess.Write,
                FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            return handle;
        }

        private static void ThrowIfStreamNotWriteable(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new ArgumentOutOfRangeException(nameof(stream), "Stream not writeable.");
        }

        #endregion
    }
}