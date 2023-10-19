using System.Diagnostics.CodeAnalysis;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum CueSheetFileType
{
    Binary,
    Motorola,
    Audio,
    Aiff,
    Flac,
    Mp3,
    Wave
}