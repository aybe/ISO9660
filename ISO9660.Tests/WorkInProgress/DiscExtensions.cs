﻿using ISO9660.Tests.FileSystem;

namespace ISO9660.Tests.WorkInProgress;

public static class DiscExtensions
{
    public static void ReadFile(this Disc disc, IsoFileSystemEntryFile file, DiscReadFileMode mode, Stream stream)
    {
        var position = file.Position;

        var track = disc.Tracks.FirstOrDefault(s => position >= s.Position)
                    ?? throw new InvalidOperationException("Failed to determine track for file.");

        var length = file.Length;

        var sectors = Convert.ToUInt32(Math.Ceiling((double)length / track.Sector.GetUserDataLength()));

        for (var i = position; i < position + sectors; i++)
        {
            var sector = track.ReadSector(i);

            var span = mode switch
            {
                DiscReadFileMode.Raw => sector.AsByteSpan(),
                DiscReadFileMode.Usr => sector.GetUserData(),
                _                => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            var size = mode == DiscReadFileMode.Usr
                ? Math.Min(Math.Max(Convert.ToInt32(length - stream.Length), 0), span.Length)
                : span.Length;

            stream.Write(span[..size]);
        }
    }
}