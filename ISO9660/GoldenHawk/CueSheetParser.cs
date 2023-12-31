﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace ISO9660.GoldenHawk;

public static partial class CueSheetParser
// https://www.gnu.org/software/ccd2cue/manual/html_node/CUE-sheet-format.html#CUE-sheet-format
// https://web.archive.org/web/20070614044112/http://www.goldenhawk.com/download/cdrwin.pdf
// https://github.com/libyal/libodraw/blob/main/documentation/CUE%20sheet%20format.asciidoc
// TODO CDTEXTFILE
// TODO SONGWRITER
// TODO POSTGAP
{
    public static CueSheet Parse(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream, Encoding.Default, true);

        var context = new CueSheetParserContext
        {
            Sheet = new CueSheet(), SheetDirectory = Path.GetDirectoryName(path)
        };

        context.Push(context.Sheet);

        while (true)
        {
            var text = reader.ReadLine();

            if (text is null)
            {
                break;
            }

            context.Text = text;

            context.TextIndent = text.TakeWhile(char.IsWhiteSpace).Count();

            context.TextLine++;

            var handled = false;

            foreach (var pair in Dictionary)
            {
                var regex = pair.Value();

                context.TextMatch = regex.Match(context.Text);

                handled = context.TextMatch.Success;

                if (handled is false)
                {
                    continue;
                }

                pair.Key(context);

                break;
            }

            if (handled is false)
            {
                throw new InvalidDataException($"Unexpected value at line {context.TextLine}: {context.Text.Trim()}");
            }
        }

        return context.Sheet;
    }

    private static void ThrowIfNotNull<T>(
        CueSheetParserContext context, [NotNull] T? value, [CallerArgumentExpression(nameof(value))] string valueName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
            return;
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.
        }

        var message = $"""The value "{context.Text.Trim()}" at line {context.TextLine} expects '{valueName}' to be 'null'.""";

        throw new InvalidDataException(message);
    }
}

public static partial class CueSheetParser
{
    private const RegexOptions HandlerRegexOptions = RegexOptions.Compiled | RegexOptions.Singleline;

    private static T Parse<T>(Capture capture, Func<string, T> func)
    {
        return func(capture.Value);
    }

    [GeneratedRegex("""^\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex WhiteSpaceRegex();

    [GeneratedRegex("""^\s*CATALOG\s+(\d{13})\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex CatalogRegex();

    [GeneratedRegex("""^\s*FILE\s+"?(.*?)"?\s+(BINARY|MOTORLA|AUDIO|AIFF|FLAC|MP3|WAVE)\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex FileRegex();

    [GeneratedRegex("""^\s*FLAGS(?:\s+(4CH|PRE|SCMS|DCP))+\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex FlagsRegex();

    [GeneratedRegex("""^\s*INDEX\s+(0?[0-9]|[1-9][0-9])\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex IndexRegex();

    [GeneratedRegex("""^\s*PERFORMER\s+"?(.*?)"?\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex PerformerRegex();

    [GeneratedRegex("""^\s*PREGAP\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex PreGapRegex();

    [GeneratedRegex("""^\s*REM\s+(.*)\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex RemRegex();

    [GeneratedRegex("""^\s*TITLE\s+"?(.*?)"?\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex TitleRegex();

    [GeneratedRegex("""^\s*TRACK\s+(0?[0-9]|[1-9][0-9])\s+(AUDIO|CDG|MODE1/2048|MODE1/2352|MODE2/2048|MODE2/2324|MODE2/2336|MODE2/2352|CDI/2336|CDI/2352)\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex TrackRegex();

    [GeneratedRegex("""^\s*ISRC\s+(\w{5}\d{7})\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex IsrcRegex();

    [GeneratedRegex("""^\s*;.*\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex CommentRegex();
}

public static partial class CueSheetParser
{
    private static readonly IReadOnlyDictionary<Action<CueSheetParserContext>, Func<Regex>> Dictionary =
        new Dictionary<Action<CueSheetParserContext>, Func<Regex>>
        {
            { WhiteSpaceHandler, WhiteSpaceRegex },
            { FileHandler, FileRegex },
            { CatalogHandler, CatalogRegex },
            { FlagsHandler, FlagsRegex },
            { IndexHandler, IndexRegex },
            { PerformerHandler, PerformerRegex },
            { PreGapHandler, PreGapRegex },
            { RemHandler, RemRegex },
            { TitleHandler, TitleRegex },
            { TrackHandler, TrackRegex },
            { IsrcHandler, IsrcRegex },
            { CommentHandler, CommentRegex }
        }.AsReadOnly();

    private static void WhiteSpaceHandler(CueSheetParserContext context)
    {
    }

    private static void CatalogHandler(CueSheetParserContext context)
    {
        ThrowIfNotNull(context, context.Sheet.Catalog);

        var catalog = Parse(context.TextMatch.Groups[1], ulong.Parse);

        context.Sheet.Catalog = catalog;
    }

    private static void FileHandler(CueSheetParserContext context)
    {
        var name = context.TextMatch.Groups[1].Value;
        var type = context.TextMatch.Groups[2].Value;

        var mode = type switch
        {
            "BINARY"   => CueSheetFileType.Binary,
            "MOTOROLA" => CueSheetFileType.Motorola,
            "AUDIO"    => CueSheetFileType.Audio,
            "AIFF"     => CueSheetFileType.Aiff,
            "FLAC"     => CueSheetFileType.Flac,
            "MP3"      => CueSheetFileType.Mp3,
            "WAVE"     => CueSheetFileType.Wave,
            _          => throw new InvalidDataException($"Unknown file type: {type}.")
        };

        if (Path.IsPathFullyQualified(name) is false)
        {
            name = Path.GetFullPath(Path.Combine(context.SheetDirectory ?? string.Empty, name));
        }

        var file = new CueSheetFile(context.Sheet, name, mode);

        context.Sheet.Files.Add(file);

        context.Push(file);
    }

    private static void FlagsHandler(CueSheetParserContext context)
    {
        var track = context.Peek<CueSheetTrack>();

        ThrowIfNotNull(context, track.Flags);

        foreach (var capture in context.TextMatch.Groups[1].Captures.Cast<Capture>())
        {
            var value = capture.Value;

            var flags = value switch
            {
                "4CH"  => CueSheetTrackFlags.FourChannelAudio,
                "PRE"  => CueSheetTrackFlags.PreEmphasis,
                "SCMS" => CueSheetTrackFlags.SerialCopyManagementSystem,
                "DCP"  => CueSheetTrackFlags.DigitalCopyPermitted,
                _      => throw new InvalidDataException($"Unknown track flag: {value}.")
            };

            track.Flags |= flags;
        }
    }

    private static void IndexHandler(CueSheetParserContext context)
    {
        var track = context.Peek<CueSheetTrack>();

        var i = Parse(context.TextMatch.Groups[1], byte.Parse);
        var m = Parse(context.TextMatch.Groups[2], byte.Parse);
        var s = Parse(context.TextMatch.Groups[3], byte.Parse);
        var f = Parse(context.TextMatch.Groups[4], byte.Parse);

        var index = new CueSheetTrackIndex(track, i, new MSF(m, s, f));

        if (track.Indices.Count == 0)
        {
            var zero = MSF.Min;

            if (index is not { Number: 0 or 1 } && index.Position != zero && track.Index is 1)
            {
                var message = $"""Track 1 index isn't 0 or 1 and doesn't start at {zero} for "{context.Text}".""";

                throw new InvalidDataException(message);
            }
        }
        else
        {
            var last = track.Indices.Last();

            if (index.Number != last.Number + 1)
            {
                var message = $"""Track index isn't consecutive to previous' for "{context.Text}".""";

                throw new InvalidDataException(message);
            }
        }

        track.Indices.Add(index);

        context.Push(index);
    }

    private static void PerformerHandler(CueSheetParserContext context)
    {
        var performer = context.TextMatch.Groups[1].Value;

        if (context.TryPeek<CueSheetTrack>(out var track))
        {
            ThrowIfNotNull(context, track.Performer);

            track.Performer = performer;

            return;
        }

        if (context.TryPeek<CueSheet>(out var sheet))
        {
            ThrowIfNotNull(context, sheet.Performer);

            sheet.Performer = performer;

            return;
        }

        const string message = "Failed to find parent track or sheet for performer.";

        throw new InvalidOperationException(message);
    }

    private static void PreGapHandler(CueSheetParserContext context)
    {
        var track = context.Peek<CueSheetTrack>();

        ThrowIfNotNull(context, track.PreGap);

        var m = Parse(context.TextMatch.Groups[1], byte.Parse);
        var s = Parse(context.TextMatch.Groups[2], byte.Parse);
        var f = Parse(context.TextMatch.Groups[3], byte.Parse);

        track.PreGap = new MSF(m, s, f);
    }

    private static void RemHandler(CueSheetParserContext context)
    {
        var element = context.Peek(s => s.Indent <= context.TextIndent);

        var comment = context.TextMatch.Groups[1].Value;

        element.Target.Comments.Add(comment);
    }

    private static void TitleHandler(CueSheetParserContext context)
    {
        var title = context.TextMatch.Groups[1].Value;

        if (context.TryPeek<CueSheetTrack>(out var track))
        {
            ThrowIfNotNull(context, track.Title);

            track.Title = title;

            return;
        }

        if (context.TryPeek<CueSheetFile>(out var file))
        {
            ThrowIfNotNull(context, file.Title);

            file.Title = title;

            return;
        }

        if (context.TryPeek<CueSheet>(out var sheet))
        {
            ThrowIfNotNull(context, sheet.Title);

            sheet.Title = title;

            return;
        }

        const string message = "Failed to find parent track/file/sheet for title";

        throw new InvalidOperationException(message);
    }

    private static void TrackHandler(CueSheetParserContext context)
    {
        var file = context.Peek<CueSheetFile>();

        var index = Parse(context.TextMatch.Groups[1], int.Parse);

        if (file.Tracks.Count is not 0) // first can be any index
        {
            var last = file.Tracks.Last();

            if (index != last.Index + 1)
            {
                var message = $"Track indices are not consecutive, current is {index}, previous was {last.Index}.";

                throw new InvalidDataException(message);
            }
        }

        var type = context.TextMatch.Groups[2].Value;

        var mode = type switch
        {
            "AUDIO"      => CueSheetTrackType.Audio,
            "CDG"        => CueSheetTrackType.Karaoke,
            "MODE1/2048" => CueSheetTrackType.Mode1Cooked,
            "MODE1/2352" => CueSheetTrackType.Mode1Raw,
            "MODE2/2048" => CueSheetTrackType.Mode2Form1Cooked,
            "MODE2/2324" => CueSheetTrackType.Mode2Form2Cooked,
            "MODE2/2336" => CueSheetTrackType.Mode2Mixed,
            "MODE2/2352" => CueSheetTrackType.Mode2Raw,
            "CDI/2336"   => CueSheetTrackType.InteractiveCooked,
            "CDI/2352"   => CueSheetTrackType.InteractiveRaw,
            _            => throw new InvalidDataException($"Unknown track mode: {type}.")
        };

        var track = new CueSheetTrack(file, index, mode);

        file.Tracks.Add(track);

        context.Push(track);
    }

    private static void IsrcHandler(CueSheetParserContext context)
    {
        var track = context.Peek<CueSheetTrack>();

        ThrowIfNotNull(context, track.Isrc);

        var isrc = context.TextMatch.Groups[1].Value;

        track.Isrc = isrc;
    }

    private static void CommentHandler(CueSheetParserContext context)
    {
    }
}