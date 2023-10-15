using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    public abstract int Index { get; }

    public abstract int Length { get; }

    public abstract int Position { get; }

    public abstract DiscTrackType Type { get; }

    internal Stream Stream { get; init; } = null!;

    public abstract ISector ReadSector(in int index);

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Type)}: {Type}";
    }
}