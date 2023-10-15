using WipeoutInstaller.Extensions;

namespace WipeoutInstaller.WorkInProgress;

public abstract class DiscTrack : Disposable
{
    public abstract bool Audio { get; }

    public abstract int Index { get; }

    public abstract int Length { get; }

    public abstract int Position { get; }

    public abstract ISector ReadSector(in int index);

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}, {nameof(Position)}: {Position}, {nameof(Audio)}: {Audio}";
    }
}