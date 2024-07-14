using Whatever.Extensions;

namespace ISO9660.Physical;

internal sealed class TrackFileIso : TrackFileBase
{
    public TrackFileIso(Stream stream, int index, int position)
    {
        Audio = false;

        Sector = ISector.GetSectorTypeIso(stream);

        Length = (stream.Length / Sector.Length).ToInt32();

        Stream = stream;

        Index = index;

        Position = position;
    }
}