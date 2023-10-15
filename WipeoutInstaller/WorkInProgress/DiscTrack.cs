using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    public virtual int Index { get; init; }

    public virtual int Length { get; init; }

    public virtual int Position { get; init; }

    public virtual DiscTrackType Type { get; init; }

    internal Stream Stream { get; init; } = null!;

    public abstract ISector ReadSector(in int index);

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Type)}: {Type}";
    }
}