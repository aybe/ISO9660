using System.Diagnostics;
using CDROMTools.Interop;

namespace CDROMTools
{
    /// <summary>
    ///     Represents a track in a CD-ROM.
    /// </summary>
    public sealed class CDROMTrack
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int _index;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly uint _lba;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly uint _lbaCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly CDROMTrackType _trackType;

        internal CDROMTrack(int index, uint lba, uint lbaCount, CDROMTrackType trackType)
        {
            _index = index;
            _lba = lba;
            _lbaCount = lbaCount;
            _trackType = trackType;
        }

        /// <summary>
        ///     Gets the (ONE-based) index for this instance.
        /// </summary>
        public int Index => _index;

        /// <summary>
        ///     Gets the track type for this instance.
        /// </summary>
        public CDROMTrackType TrackType => _trackType;

        /// <summary>
        ///     Gets the starting LBA for this instance.
        /// </summary>
        public uint LBA => _lba;

        /// <summary>
        ///     Gets the count of LBA blocks for this instance.
        /// </summary>
        public uint LBACount => _lbaCount;

        /// <summary>
        ///     Gets the 'minutes' part of the length for this instance.
        /// </summary>
        public uint PartMinutes => TotalMinutes - TotalHours*60;

        /// <summary>
        ///     Gets the 'seconds' part of the length for this instance.
        /// </summary>
        public uint PartSeconds => TotalSeconds - PartMinutes*60;

        /// <summary>
        ///     Gets the 'frames' part of the length for this instance.
        /// </summary>
        public uint PartFrames => _lbaCount%NativeConstants.CD_BLOCKS_PER_SECOND;

        /// <summary>
        ///     Gets the length in hours for this instance.
        /// </summary>
        public uint TotalHours => TotalMinutes/60;

        /// <summary>
        ///     Gets the length in minutes for this instance.
        /// </summary>
        public uint TotalMinutes => TotalSeconds/60;

        /// <summary>
        ///     Gets the length in seconds for this instance.
        /// </summary>
        public uint TotalSeconds => _lbaCount/NativeConstants.CD_BLOCKS_PER_SECOND;

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"Index: {Index:D2}, TrackType: {TrackType}";
        }
    }
}