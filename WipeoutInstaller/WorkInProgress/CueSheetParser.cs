﻿using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace WipeoutInstaller.WorkInProgress;

public static partial class CueSheetParser
// https://www.gnu.org/software/ccd2cue/manual/html_node/CUE-sheet-format.html#CUE-sheet-format
// https://web.archive.org/web/20070614044112/http://www.goldenhawk.com/download/cdrwin.pdf
// https://github.com/libyal/libodraw/blob/main/documentation/CUE%20sheet%20format.asciidoc
// TODO CDTEXTFILE
// TODO SONGWRITER
// TODO POSTGAP
{
    public static CueSheet Parse(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.Default, true);

        var context = new Context
        {
            Sheet = new CueSheet()
        };

        while (true)
        {
            var text = reader.ReadLine();

            if (text is null)
            {
                break;
            }

            context.Text = text;

            context.Line++;

            var handled = false;

            foreach (var pair in Handlers2)
            {
                var regex = pair.Value();

                context.Match = regex.Match(context.Text);

                handled = context.Match.Success;

                if (handled is false)
                {
                    continue;
                }

                pair.Key(context);

                break;
            }

            if (handled is false)
            {
                throw new InvalidDataException($"Unexpected value at line {context.Line}: {context.Text.Trim()}");
            }
        }

        if (context.File is null)
        {
            throw new InvalidDataException("There must be at least one file defined.");
        }

        if (context.Track is null)
        {
            throw new InvalidDataException("There must at least one track defined.");
        }

        return context.Sheet;
    }

    private static void ThrowIfNull<T>([NotNull] T? value, Context context)
    {
        if (value != null)
        {
            return;
        }

        var message = $"""The value of type {typeof(T).Name} is expected to not be 'null' for "{context.Text.Trim()}" at line {context.Line}.""";

        throw new InvalidDataException(message);
    }

    private static void ThrowIfNot<TSource, TValue>(TSource source, Expression<Func<TSource, TValue>> valueExpression, TValue expected, string input, int line)
    {
        var value = valueExpression.Compile()(source);

        if (EqualityComparer<TValue>.Default.Equals(value, expected))
        {
            return;
        }

        var expression = (MemberExpression)valueExpression.Body;
        var memberType = expression.Member.DeclaringType!.Name;
        var memberName = expression.Member.Name;

        var message = $"""The value of '{memberName}' in '{memberType}' is expected to be '{expected}' for "{input.Trim()}" at line {line}.""";

        throw new InvalidDataException(message);
    }

    private sealed class Context
    {
        public int Line { get; set; }

        public string Text { get; set; } = null!;

        public required CueSheet Sheet { get; init; }

        public CueSheetFile? File { get; set; }

        public CueSheetTrack? Track { get; set; }

        public Match Match { get; set; } = null!;
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

    [GeneratedRegex("""^\s*FILE\s+"?(.*)"?\s+(BINARY|MOTORLA|AUDIO|AIFF|FLAC|MP3|WAVE)\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex FileRegex();

    [GeneratedRegex("""^\s*FLAGS(?:\s+(4CH|PRE|SCMS|DCP))+\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex FlagsRegex();

    [GeneratedRegex("""^\s*INDEX\s+(0?[0-9]|[1-9][0-9])\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex IndexRegex();

    [GeneratedRegex("""^\s*PERFORMER\s+"?(.*)"?\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex PerformerRegex();

    [GeneratedRegex("""^\s*PREGAP\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex PreGapRegex();

    [GeneratedRegex("""^\s*REM\s+(.*)\s*\r?$""", HandlerRegexOptions)]
    private static partial Regex RemRegex();

    [GeneratedRegex("""^\s*TITLE\s+"?(.*)"?\s*\r?$""", HandlerRegexOptions)]
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
    private static readonly IReadOnlyDictionary<Action<Context>, Func<Regex>> Handlers2 =
        new Dictionary<Action<Context>, Func<Regex>>
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

    private static void WhiteSpaceHandler(Context context)
    {
    }

    private static void CatalogHandler(Context context)
    {
        ThrowIfNot(context.Sheet, s => s.Catalog, null, context.Text, context.Line);

        var catalog = Parse(context.Match.Groups[1], ulong.Parse);

        context.Sheet.Catalog = catalog;
    }

    private static void FileHandler(Context context)
    {
        var name = context.Match.Groups[1].Value;
        var type = context.Match.Groups[2].Value;

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

        context.File = new CueSheetFile(name, mode);

        context.Sheet.Files.Add(context.File);
    }

    private static void FlagsHandler(Context context)
    {
        ThrowIfNull(context.Track, context);

        ThrowIfNot(context.Track, s => s.Flags, CueSheetTrackFlags.None, context.Text, context.Line);

        foreach (var capture in context.Match.Groups[1].Captures.Cast<Capture>())
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

            context.Track.Flags |= flags;
        }
    }

    private static void IndexHandler(Context context)
    {
        ThrowIfNull(context.Track, context);

        var i = Parse(context.Match.Groups[1], byte.Parse);
        var m = Parse(context.Match.Groups[2], byte.Parse);
        var s = Parse(context.Match.Groups[3], byte.Parse);
        var f = Parse(context.Match.Groups[4], byte.Parse);

        var index = new CueSheetTrackIndex(i, new Msf(m, s, f));

        if (context.Track.Indices.Count == 0)
        {
            var zero = Msf.Zero;

            if (index is not { Number: 0 or 1 } && index.Position != zero && context.Track.Index is 1)
            {
                var message = $"""Track 1 index isn't 0 or 1 and doesn't start at {zero} for "{context.Text}".""";

                throw new InvalidDataException(message);
            }
        }
        else
        {
            var last = context.Track.Indices.Last();

            if (index.Number != last.Number + 1)
            {
                var message = $"""Track index isn't consecutive to previous' for "{context.Text}".""";

                throw new InvalidDataException(message);
            }
        }

        context.Track.Indices.Add(index);
    }

    private static void PerformerHandler(Context context)
    {
        var performer = context.Match.Groups[1].Value;

        if (context.Track == null)
        {
            ThrowIfNot(context.Sheet, s => s.Performer, null, context.Text, context.Line);

            context.Sheet.Performer = performer;
        }
        else
        {
            ThrowIfNot(context.Track, s => s.Performer, null, context.Text, context.Line);

            context.Track.Performer = performer;
        }
    }

    private static void PreGapHandler(Context context)
    {
        ThrowIfNull(context.Track, context);

        ThrowIfNot(context.Track, s => s.PreGap, null, context.Text, context.Line);

        var m = Parse(context.Match.Groups[1], byte.Parse);
        var s = Parse(context.Match.Groups[2], byte.Parse);
        var f = Parse(context.Match.Groups[3], byte.Parse);

        context.Track.PreGap = new Msf(m, s, f);
    }

    private static void RemHandler(Context context)
    {
        var comment = context.Match.Groups[1].Value;

        context.Sheet.Comments.Add(comment);
    }

    private static void TitleHandler(Context context)
    {
        var title = context.Match.Groups[1].Value;

        if (context.Track == null)
        {
            ThrowIfNot(context.Sheet, s => s.Title, null, context.Text, context.Line);

            context.Sheet.Title = title;
        }
        else
        {
            ThrowIfNot(context.Track, s => s.Title, null, context.Text, context.Line);

            context.Track.Title = title;
        }
    }

    private static void TrackHandler(Context context)
    {
        ThrowIfNull(context.File, context);

        var index = Parse(context.Match.Groups[1], int.Parse);

        if (context.File.Tracks.Count is not 0) // first can be any index
        {
            var last = context.File.Tracks.Last();

            if (index != last.Index + 1)
            {
                var message = $"Track indices are not consecutive, current is {index}, previous was {last.Index}.";

                throw new InvalidDataException(message);
            }
        }

        var type = context.Match.Groups[2].Value;

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

        context.Track = new CueSheetTrack(index, mode);

        context.File.Tracks.Add(context.Track);
    }

    private static void IsrcHandler(Context context)
    {
        ThrowIfNull(context.Track, context);

        ThrowIfNot(context.Track, s => s.Isrc, null, context.Text, context.Line);

        var isrc = context.Match.Groups[1].Value;

        context.Track.Isrc = isrc;
    }

    private static void CommentHandler(Context context)
    {
    }
}