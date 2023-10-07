using System;
using System.IO;
using System.Runtime.InteropServices;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    [StructLayout(LayoutKind.Sequential)] // variable-sized
    public struct PathTableRecord
    {
        // NOTE : Tested against an Type L Path Table only !

        public readonly Iso711 DirectoryIdentifierLength;
        public readonly Iso711 ExtendedAttributeRecordLength;
        public readonly Iso731 ExtentLocation;
        public readonly Iso721 ParentDirectoryNumber;
        public readonly string DirectoryIdentifier;

        public PathTableRecord(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            DirectoryIdentifierLength = new Iso711(reader);
            ExtendedAttributeRecordLength = new Iso711(reader);
            ExtentLocation = new Iso731(reader);
            ParentDirectoryNumber = new Iso721(reader);
            DirectoryIdentifier = reader.ReadStringAscii(DirectoryIdentifierLength);
            if (DirectoryIdentifier == "\0") // keep it 'friendly'
                DirectoryIdentifier = @"\";
            if (DirectoryIdentifierLength % 2 != 0) // keep it 'happy'
            {
                var padding = reader.ReadByte();
            }
        }

        public override string ToString()
        {
            return $"{DirectoryIdentifier}, {ExtentLocation.Value}";
        }
    }
}