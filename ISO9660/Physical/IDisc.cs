namespace ISO9660.Physical;

public interface IDisc : IDisposable, IAsyncDisposable
{
    IReadOnlyList<ITrack> Tracks { get; }
}