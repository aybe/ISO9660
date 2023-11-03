﻿using ISO9660.CDRWIN;

namespace ISO9660.WorkInProgress;

public sealed class TrackCue : Track
{
    private const int PreGapSize = 150; // TODO move

    public TrackCue(CueSheetTrack track)
    {
        Stream = File.OpenRead((Track = track).File.Name);
    }

    private Stream Stream { get; }

    private CueSheetTrack Track { get; }

    public override bool Audio => Track.Type == CueSheetTrackType.Audio;

    public override int Index => Track.Index;

    public override int Length => GetLength(Track, Sector.Length);

    public override int Position => GetPosition(Track, Sector.Length);

    public override ISector Sector
    {
        get
        {
            var type = Track.Type;

            ISector sector = type switch
            {
                CueSheetTrackType.Audio             => new SectorRawAudio(),
                CueSheetTrackType.Karaoke           => throw new NotSupportedException(type.ToString()),
                CueSheetTrackType.Mode1Cooked       => new SectorCooked2048(),
                CueSheetTrackType.Mode1Raw          => new SectorRawMode1(),
                CueSheetTrackType.Mode2Form1Cooked  => new SectorCooked2324(),
                CueSheetTrackType.Mode2Form2Cooked  => new SectorCooked2336(),
                CueSheetTrackType.Mode2Mixed        => throw new NotSupportedException(type.ToString()),
                CueSheetTrackType.Mode2Raw          => new SectorRawMode2Form1(),
                CueSheetTrackType.InteractiveCooked => throw new NotSupportedException(type.ToString()),
                CueSheetTrackType.InteractiveRaw    => throw new NotSupportedException(type.ToString()),
                _                                   => throw new NotSupportedException(type.ToString())
            };

            return sector;
        }
    }

    protected override void DisposeManaged()
    {
        Stream.Dispose();
    }

    public override ISector ReadSector(in int index)
    {
        if (index >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        var length = Sector.Length;

        Stream.Position = index * length;

        var type = Track.Type;

        var sector = type switch // TODO implement other track types
        {
            CueSheetTrackType.Mode1Cooked => ISector.Read<SectorCooked2048>(Stream),
            CueSheetTrackType.Mode1Raw    => ISector.Read<SectorRawMode1>(Stream),
            CueSheetTrackType.Mode2Raw    => ISector.Read<SectorRawMode2Form1>(Stream),
            _                             => throw new NotSupportedException($"Track mode not supported: {type}.")
        };

        return sector;
    }

    private static int GetLength(CueSheetTrack track, in int sectorSize)
    {
        var lengthStream = Convert.ToInt32(new FileInfo(track.File.Name).Length / sectorSize); // BUG fetch correct sector size

        var length = 0;

        var files = track.File.Sheet.Files;

        if (files.Count == 1)
        {
            var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

            for (var node = tracks.First; node != null; node = node.Next)
            {
                var prev = node.Previous;
                var next = node.Next;

                var value = node.Value;

                var pos1 = GetPosition(value, sectorSize);
                var pos2 = next != null ? GetPosition(next.Value, sectorSize) : lengthStream;

                length = pos2 - pos1;

                var a = value.Type is not CueSheetTrackType.Audio &&
                        prev != null && prev.Value.Type != value.Type; // 6.32.3.18 SCSI MMC-5

                var b = value.Type is not CueSheetTrackType.Audio &&
                        next != null && next.Value.Type != value.Type; // 6.32.3.19 SCSI MMC-5

                var c = next != null && value.Index0 != null; // more implicit convention crap

                if (a || b)
                {
                    length -= PreGapSize;
                }

                if (c)
                {
                    length -= PreGapSize;
                }

                if (value == track)
                {
                    break;
                }
            }
        }
        else
        {
            length = lengthStream - PreGapSize - track.Index1.Position.ToLBA();
        }

        return length;
    }

    private static int GetPosition(CueSheetTrack track, in int sectorSize)
    {
        var files = track.File.Sheet.Files;

        var tracks = new LinkedList<CueSheetTrack>(files.SelectMany(s => s.Tracks));

        var position = -PreGapSize; // MSF 00:00.00 is LBA -150

        for (var node = tracks.First; node != null; node = node.Next)
        {
            var value = node.Value;

            if (files.Count == 1)
            {
                position = value.Indices[0].Position.ToLBA();

                if (value == track)
                {
                    break;
                }
            }
            else
            {
                if (value == track)
                {
                    break;
                }

                position += Convert.ToInt32(new FileInfo(value.File.Name).Length / sectorSize);
            }
        }

        for (var node = tracks.First; node != null; node = node.Next) // ECMA-130 - 20.2 User Data Area
        {
            var value = node.Value;

            var a = value.Index is 1;
            var b = value.Index is not 1 && node.Previous is { } previous && previous.Value.Type != value.Type;
            var c = value.Type is not CueSheetTrackType.Audio && node.Next is { Value.Type: CueSheetTrackType.Audio };

            if (a || b || c)
            {
                position += PreGapSize;
            }

            if (value == track)
            {
                break;
            }
        }

        return position;
    }
}