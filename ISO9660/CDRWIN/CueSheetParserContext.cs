using System.Text.RegularExpressions;

namespace ISO9660.CDRWIN;

internal sealed class CueSheetParserContext
{
    public string Text { get; set; } = string.Empty;

    public int TextIndent { get; set; }

    public int TextLine { get; set; }

    public required CueSheet Sheet { get; init; }

    public required string? Directory { get; init; }

    private Stack<CueSheetParserElement> ElementStack { get; } = new();

    public CueSheetFile? File { get; set; }

    public CueSheetTrack? Track { get; set; }

    public Match Match { get; set; } = null!;

    public T Peek<T>() where T : CueSheetElement
    {
        var element = ElementStack.FirstOrDefault(s => s.Target is T)
                      ?? throw new InvalidOperationException($"Failed to find a parent of type {typeof(T).Name}.");

        var target = (element.Target as T)!;

        return target;
    }

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