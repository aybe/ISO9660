using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.Iso9660;

public sealed class DirectoryRecord
{
    public DirectoryRecord(BinaryReader reader)
    {
        var stream = reader.BaseStream;

        var position = stream.Position;

        LengthOfDirectoryRecord = new Iso711(reader);

        ExtendedAttributeRecordLength = new Iso711(reader);

        LocationOfExtent = new Iso733(reader);

        DataLength = new Iso733(reader);

        RecordingDateAndTime = new RecordingDateAndTime(reader);

        FileFlags = reader.Read<FileFlags>();

        FileUnitSize = new Iso711(reader);

        InterleaveGapSize = new Iso711(reader);

        VolumeSequenceNumber = new Iso723(reader);

        LengthOfFileIdentifier = new Iso711(reader);

        FileIdentifier = reader.ReadBytes(LengthOfFileIdentifier); // TODO, outside this

        PaddingField = LengthOfFileIdentifier % 2 is 0
            ? reader.ReadByte()
            : null;

        var drLen = stream.Position - position;
        var suLen = (int)(LengthOfDirectoryRecord - drLen);

        SystemUse = reader.ReadBytes(suLen);
    }

    public Iso711 LengthOfDirectoryRecord { get; }

    public Iso711 ExtendedAttributeRecordLength { get; }

    public Iso733 LocationOfExtent { get; }

    public Iso733 DataLength { get; }

    public RecordingDateAndTime RecordingDateAndTime { get; }

    public FileFlags FileFlags { get; }

    public Iso711 FileUnitSize { get; }

    public Iso711 InterleaveGapSize { get; }

    public Iso723 VolumeSequenceNumber { get; }

    public Iso711 LengthOfFileIdentifier { get; }

    public byte[] FileIdentifier { get; }

    public byte? PaddingField { get; }

    public byte[] SystemUse { get; }
}