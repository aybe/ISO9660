using System;
using System.IO;
using CDROMTools.Utils;

namespace CDROMTools.Iso9660
{
    public sealed class SupplementaryVolumeDescriptor : VolumeDescriptor
    {
        public readonly string AbstractFileIdentifier;
        public readonly string ApplicationIdentifier;
        public readonly byte[] ApplicationUse;
        public readonly string BibliographicFileIdentifier;
        public readonly string CopyrightFileIdentifier;
        public readonly string DataPreparerSetIdentifier;
        public readonly DirectoryRecord DirectoryRecord;
        public readonly byte[] EscapeSequences;
        public readonly Iso711 FileStructureVersion;
        public readonly Iso731 LocationOptionalTypeLPathTable;
        public readonly Iso732 LocationOptionalTypeMPathTable;
        public readonly Iso731 LocationTypeLPathTable;
        public readonly Iso732 LocationTypeMPathTable;
        public readonly Iso723 LogicalBlockSize;
        public readonly Iso733 PathTableSize;
        public readonly string PublisherSetIdentifier;
        public readonly byte Reserved1;
        public readonly byte[] Reserved2;
        public readonly string SystemIdentifier;
        public readonly byte[] Unused1;
        public readonly IsoLongDateTime VolumeCreationDate;
        public readonly IsoLongDateTime VolumeEffectiveDate;
        public readonly IsoLongDateTime VolumeExpirationDate;
        public readonly VolumeFlags VolumeFlags;
        public readonly string VolumeIdentifier;
        public readonly IsoLongDateTime VolumeModificationDate;
        public readonly Iso723 VolumeSequenceNumber;
        public readonly string VolumeSetIdentifier;
        public readonly Iso723 VolumeSetSize;
        public readonly Iso733 VolumeSpaceSize;

        internal SupplementaryVolumeDescriptor(VolumeDescriptor descriptor, BinaryReader reader) : base(descriptor)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            VolumeFlags = (VolumeFlags) reader.ReadByte();
            SystemIdentifier = reader.ReadStringAscii(32).Trim();
            VolumeIdentifier = reader.ReadStringAscii(32).Trim();
            Unused1 = reader.ReadBytes(8);
            VolumeSpaceSize = new Iso733(reader);
            EscapeSequences = reader.ReadBytes(32);
            VolumeSetSize = new Iso723(reader);
            VolumeSequenceNumber = new Iso723(reader);
            LogicalBlockSize = new Iso723(reader);
            PathTableSize = new Iso733(reader);
            LocationTypeLPathTable = new Iso731(reader);
            LocationOptionalTypeLPathTable = new Iso731(reader);
            LocationTypeMPathTable = new Iso732(reader);
            LocationOptionalTypeMPathTable = new Iso732(reader);
            DirectoryRecord = new DirectoryRecord(reader);
            VolumeSetIdentifier = reader.ReadStringAscii(128).Trim();
            PublisherSetIdentifier = reader.ReadStringAscii(128).Trim();
            DataPreparerSetIdentifier = reader.ReadStringAscii(128).Trim();
            ApplicationIdentifier = reader.ReadStringAscii(128).Trim();
            CopyrightFileIdentifier = reader.ReadStringAscii(37).Trim();
            AbstractFileIdentifier = reader.ReadStringAscii(37).Trim();
            BibliographicFileIdentifier = reader.ReadStringAscii(37).Trim();
            VolumeCreationDate = new IsoLongDateTime(reader);
            VolumeModificationDate = new IsoLongDateTime(reader);
            VolumeExpirationDate = new IsoLongDateTime(reader);
            VolumeEffectiveDate = new IsoLongDateTime(reader);
            FileStructureVersion = new Iso711(reader);
            Reserved1 = reader.ReadByte();
            ApplicationUse = reader.ReadBytes(512);
            Reserved2 = reader.ReadBytes(653);
        }
    }
}