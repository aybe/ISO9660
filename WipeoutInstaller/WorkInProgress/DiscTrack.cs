using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    public int Index { get; init; }

    public MSF Position { get; init; } // TODO make position absolute (for multi-file cue/bin)

    public DiscTrackType Type { get; init; }

    internal Stream Stream { get; init; } = null!;

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Type)}: {Type}";
    }

    public abstract ISector ReadSector(int index);
}