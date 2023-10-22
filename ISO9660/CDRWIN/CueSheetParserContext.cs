﻿using System.Text.RegularExpressions;

namespace ISO9660.CDRWIN;

internal sealed class CueSheetParserContext
{
    public int Line { get; set; }

    public string Text { get; set; } = string.Empty;

    public required CueSheet Sheet { get; init; }

    public required string? Directory { get; init; }

    public int Indent { get; set; }

    private Stack<CueSheetParserElement> ElementStack { get; } = new();

    public CueSheetFile? File { get; set; }

    public CueSheetTrack? Track { get; set; }

    public Match Match { get; set; } = null!;

    public CueSheetParserElement Peek(Func<CueSheetParserElement, bool> predicate)
    {
        var element = ElementStack.First(predicate);

        return element;
    }

    public void Push(CueSheetElement target)
    {
        var indent = Text.TakeWhile(char.IsWhiteSpace).Count();

        ElementStack.Push(new CueSheetParserElement(indent, target));
    }
}