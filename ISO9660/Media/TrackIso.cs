﻿using Whatever.Extensions;

namespace ISO9660.Media;

public sealed class TrackIso : Track
{
    public TrackIso(Stream stream, int index, int position)
    {
        ISector[] sectors =
        {
            new SectorCooked2048(),
            new SectorCooked2324(),
            new SectorCooked2336()
        };

        Sector = sectors.Single(s => stream.Length % s.GetUserDataLength() == 0);

        Length = (stream.Length / Sector.Length).ToInt32();

        Stream = stream;

        Index = index;

        Position = position;
    }

    private Stream Stream { get; }

    public override bool Audio { get; } = false;

    public override int Index { get; }

    public override int Length { get; }

    public override int Position { get; }

    public override ISector Sector { get; }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(in int index)
    {
        Stream.Position = index * Sector.Length;

        var sector = Sector switch
        {
            SectorCooked2048 => ISector.Read<SectorCooked2048>(Stream),
            SectorCooked2324 => ISector.Read<SectorCooked2324>(Stream),
            SectorCooked2336 => ISector.Read<SectorCooked2336>(Stream),
            _                => throw new InvalidDataException()
        };

        return sector;
    }
}