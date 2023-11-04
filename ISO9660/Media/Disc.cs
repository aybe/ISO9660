﻿using Whatever.Extensions;

namespace ISO9660.Media;

public sealed class Disc : Disposable
{
    public IList<Track> Tracks { get; } = new List<Track>();

    protected override void DisposeManaged()
    {
        foreach (var track in Tracks)
        {
            track.Dispose();
        }
    }
}