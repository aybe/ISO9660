using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    protected DiscTrack(Disc disc)
    {
        Disc = disc;
    }

    protected Disc Disc { get; }

    public int Index { get; init; }

    public MSF Position { get; init; } // TODO make position absolute (for multi-file cue/bin)

    public DiscTrackType Type { get; init; }

    internal Stream Stream { get; init; } = null!;

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Type)}: {Type}";
    }

    public abstract ISector ReadSector(int index);

    public abstract int GetPosition();
}