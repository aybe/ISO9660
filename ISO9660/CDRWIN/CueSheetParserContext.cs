using System.Text.RegularExpressions;

namespace ISO9660.CDRWIN;

internal sealed class CueSheetParserContext
{
    public int Line { get; set; }

    public string Text { get; set; } = null!;

    public required CueSheet Sheet { get; init; }

    public required string? Directory { get; init; }

    public CueSheetFile? File { get; set; }

    public CueSheetTrack? Track { get; set; }

    public Match Match { get; set; } = null!;
}