using Whatever.Extensions;

namespace ISO9660.Logical;

public sealed class DirectoryRecord
{
    public DirectoryRecord(Stream stream)
    {
        var position = stream.Position;

        LengthOfDirectoryRecord = stream.ReadIso711();

        if (LengthOfDirectoryRecord == 0)
        {
            return;
        }

        ExtendedAttributeRecordLength = stream.ReadIso711();

        LocationOfExtent = stream.ReadIso733();

        DataLength = stream.ReadIso733();

        RecordingDateAndTime = new DirectoryRecordDateTime(stream);

        FileFlags = stream.Read<DirectoryRecordFlags>();

        FileUnitSize = stream.ReadIso711();

        InterleaveGapSize = stream.ReadIso711();

        VolumeSequenceNumber = stream.ReadIso723();

        LengthOfFileIdentifier = stream.ReadIso711();

        FileIdentifier = stream.ReadIsoString(LengthOfFileIdentifier,
            IsoStringFlags.DCharacters | IsoStringFlags.Separator1 | IsoStringFlags.Separator2 | IsoStringFlags.Byte00 | IsoStringFlags.Byte01);

        PaddingField = LengthOfFileIdentifier % 2 is 0
            ? stream.ReadByte().ToByte()
            : null;

        var drLen = stream.Position - position;
        var suLen = (int)(LengthOfDirectoryRecord - drLen);

        SystemUse = stream.ReadExactly(suLen);
    }

    public byte LengthOfDirectoryRecord { get; }

    public byte ExtendedAttributeRecordLength { get; }

    public uint LocationOfExtent { get; }

    public uint DataLength { get; }

    public DirectoryRecordDateTime RecordingDateAndTime { get; } = null!;

    public DirectoryRecordFlags FileFlags { get; }

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