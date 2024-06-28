namespace ISO9660.WorkInProgress;

public sealed record Track(byte Number, int Position, int Length, TrackType Type);