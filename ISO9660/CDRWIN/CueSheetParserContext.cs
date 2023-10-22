using System.Text.RegularExpressions;

namespace ISO9660.CDRWIN;

internal sealed class CueSheetParserContext
{
    public string Text { get; set; } = string.Empty;

    public int TextIndent { get; set; }

    public int TextLine { get; set; }

    public Match TextMatch { get; set; } = null!;

    public required CueSheet Sheet { get; init; }

    public required string? Directory { get; init; }

    private Stack<CueSheetParserElement> ElementStack { get; } = new();

    public T Peek<T>() where T : CueSheetElement
    {
        if (!TryPeek(out T result))
        {
            throw new InvalidOperationException($"Failed to find a parent of type {typeof(T).Name}.");
        }

        return result;
    }

    public CueSheetParserElement Peek(Func<CueSheetParserElement, bool> predicate)
    {
        var element = ElementStack.First(predicate);

        return element;
    }

    public bool TryPeek<T>(out T result) where T : CueSheetElement
    {
        result = default!;

        var element = ElementStack.FirstOrDefault(s => s.Target is T);

        if (element == null)
        {
            return false;
        }

        result = (element.Target as T)!;

        return true;
    }

    public void Push(CueSheetElement target)
    {
        var indent = Text.TakeWhile(char.IsWhiteSpace).Count();

        ElementStack.Push(new CueSheetParserElement(indent, target));
    }
}