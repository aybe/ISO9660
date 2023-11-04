using Whatever.Extensions;

namespace ISO9660.FileSystem;

public sealed class DirectoryRecord
{
    public DirectoryRecord(BinaryReader reader)
    {
        var stream = reader.BaseStream;

        var position = stream.Position;

        LengthOfDirectoryRecord = reader.ReadIso711();

        if (LengthOfDirectoryRecord == 0)
        {
            return;
        }

        ExtendedAttributeRecordLength = reader.ReadIso711();

        LocationOfExtent = reader.ReadIso733();

        DataLength = reader.ReadIso733();

        RecordingDateAndTime = new DirectoryRecordDateTime(reader);

        FileFlags = reader.Read<DirectoryRecordFileFlags>();

        FileUnitSize = reader.ReadIso711();

        InterleaveGapSize = reader.ReadIso711();

        VolumeSequenceNumber = reader.ReadIso723();

        LengthOfFileIdentifier = reader.ReadIso711();

        FileIdentifier = reader.ReadIsoString(LengthOfFileIdentifier,
            IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2 | IsoStringFlags.Byte00 | IsoStringFlags.Byte01);

        PaddingField = LengthOfFileIdentifier % 2 is 0
            ? reader.ReadByte()
            : null;

        var drLen = stream.Position - position;
        var suLen = (int)(LengthOfDirectoryRecord - drLen);

        SystemUse = reader.ReadBytes(suLen);
    }

    public byte LengthOfDirectoryRecord { get; }

    public byte ExtendedAttributeRecordLength { get; }

    public uint LocationOfExtent { get; }

    public uint DataLength { get; }

    public DirectoryRecordDateTime RecordingDateAndTime { get; }

    public DirectoryRecordFileFlags FileFlags { get; }

    public byte FileUnitSize { get; }

    public byte InterleaveGapSize { get; }

    public ushort VolumeSequenceNumber { get; }

    public byte LengthOfFileIdentifier { get; }

    public string FileIdentifier { get; } = null!;

    public byte? PaddingField { get; }

    public byte[] SystemUse { get; } = null!;

    public override string ToString()
    {
        return $"{nameof(FileFlags)}: {FileFlags}, " +
               $"{nameof(FileIdentifier)}: {FileIdentifier}, " +
               $"{nameof(DataLength)}: {DataLength}, " +
               $"{nameof(LocationOfExtent)}: {LocationOfExtent}";
    }
}