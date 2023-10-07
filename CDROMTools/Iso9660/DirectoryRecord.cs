using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential)] // this is a variable-sized struct
    public struct DirectoryRecord
    {
        public readonly Iso711 DirectoryRecordLength;
        public readonly Iso711 ExtendedAttributeRecordLength;
        public readonly Iso733 ExtentLocation;
        public readonly Iso733 DataLength;
        public readonly IsoShortDateTime RecordingDateTime;
        public readonly FileFlags FileFlags;
        public readonly Iso711 FileUnitSize;
        public readonly Iso711 InterleaveGapSize;
        public readonly Iso723 VolumeSequenceNumber;
        public readonly string FileIdentifier;

        public DirectoryRecord(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            DirectoryRecordLength = new Iso711(reader);
            if (DirectoryRecordLength <= 0) // do not try further parsing if true
            {
                ExtendedAttributeRecordLength = new Iso711();
                ExtentLocation = new Iso733();
                DataLength = new Iso733();
                RecordingDateTime = new IsoShortDateTime();
                FileFlags = 0;
                FileUnitSize = new Iso711();
                InterleaveGapSize = new Iso711();
                VolumeSequenceNumber = new Iso723();
                FileIdentifier = null;
                return;
            }

            ExtendedAttributeRecordLength = new Iso711(reader);

            ExtentLocation = new Iso733(reader);

            DataLength = new Iso733(reader);

            RecordingDateTime = new IsoShortDateTime(reader);

            FileFlags = (FileFlags) reader.ReadByte();

            FileUnitSize = new Iso711(reader);

            InterleaveGapSize = new Iso711(reader);

            VolumeSequenceNumber = new Iso723(reader);

            var fileIdentifierLength = new Iso711(reader);
            var fileIdentifier = reader.ReadBytes(fileIdentifierLength);
            FileIdentifier = Encoding.ASCII.GetString(fileIdentifier);
            if (fileIdentifier.Length == 1) // if this is a 'special' path, make it 'friendly'
            {
                var b = fileIdentifier[0];
                if (b == 0)
                {
                    FileIdentifier = ".";
                }
                else if (b == 1)
                {
                    FileIdentifier = "..";
                }
            }

            // skip these for now ...
            var silly = DirectoryRecordLength - 33 - fileIdentifierLength;
            if (silly > 0)
            {
                var bytes = reader.ReadBytes(silly);
            }
        }

        public override string ToString()
        {
            return $"'{FileIdentifier}', {FileFlags}, {DataLength.Value}";
        }
    }
}