namespace ISO9660;

public interface ITrack : IDisposable
{
    int Index { get; }

    int Position { get; }

    int Length { get; }

    bool Audio { get; }
}