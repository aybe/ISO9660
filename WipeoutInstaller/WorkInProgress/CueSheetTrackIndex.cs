namespace WipeoutInstaller.WorkInProgress;

public class CueSheetTrackIndex
{
    public CueSheetTrackIndex(int number, int minutes, int seconds, int frames)
    {
        Number  = number;
        Minutes = minutes;
        Seconds = seconds;
        Frames  = frames;
    }

    public int Number { get; }

    public int Minutes { get; }

    public int Seconds { get; }

    public int Frames { get; }
}