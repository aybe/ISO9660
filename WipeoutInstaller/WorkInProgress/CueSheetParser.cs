using System.Diagnostics.CodeAnalysis;
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

        var sheet = new CueSheet();

        var file = default(CueSheetFile?);

        var track = default(CueSheetTrack?);

        while (true)
        {
            var line = reader.ReadLine();

            if (line is null)
            {
                break;
            }

            if (!Handlers.Any(handler => handler(line, sheet, ref file, ref track)))
            {
                throw new NotSupportedException(line);
            }
        }

        if (file is null)
        {
            throw new InvalidDataException("There must be at least one file defined.");
        }

        if (track is null)
        {
            throw new InvalidDataException("There must at least one track defined.");
        }

        return sheet;
    }

    private static void ThrowIfNull<T>([NotNull] T? value, string input)
    // TODO tell line at which it occurred
    {
        if (value != null)
        {
            return;
        }

        var message = $"""There is no parent of type {typeof(T).Name} for "{input.Trim()}".""";

        throw new InvalidDataException(message);
    }

    private static void ThrowIfNot<TSource, TValue>(
        TSource source, Expression<Func<TSource, TValue>> valueExpression, TValue expected, string input)
    {
        var value = valueExpression.Compile()(source);

        if (EqualityComparer<TValue>.Default.Equals(value, expected))
        {
            return;
        }

        var expression = (MemberExpression)valueExpression.Body;
        var memberType = expression.Member.DeclaringType!.Name;
        var memberName = expression.Member.Name;

        var message = $"""The value of '{memberName}' in '{memberType}' is expected to be '{expected}' for "{input.Trim()}".""";

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

    private static bool TryMatch(in Regex regex, in string input, out Match match)
    {
        match = regex.Match(input);

        var success = match.Success;

        return success;
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
    private static readonly HashSet<CueSheetHandler> Handlers =
        new()
        {
            WhiteSpaceHandler,
            FileHandler,
            CatalogHandler,
            FlagsHandler,
            IndexHandler,
            PerformerHandler,
            PreGapHandler,
            RemHandler,
            TitleHandler,
            TrackHandler,
            IsrcHandler,
            CommentHandler
        };

    private static bool WhiteSpaceHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        var success = TryMatch(WhiteSpaceRegex(), input, out _);

        return success;
    }

    private static bool CatalogHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(CatalogRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNot(sheet, s => s.Catalog, null, input);

        var catalog = Parse(match.Groups[1], ulong.Parse);

        sheet.Catalog = catalog;

        return true;
    }

    private static bool FileHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(FileRegex(), input, out var match))
        {
            return false;
        }

        var name = match.Groups[1].Value;
        var type = match.Groups[2].Value;

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

        file = new CueSheetFile(name, mode);

        sheet.Files.Add(file);

        return true;
    }

    private static bool FlagsHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(FlagsRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNull(track, input);

        ThrowIfNot(track, s => s.Flags, CueSheetTrackFlags.None, input);

        foreach (var capture in match.Groups[1].Captures.Cast<Capture>())
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

        return true;
    }

    private static bool IndexHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(IndexRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNull(track, input);

        var i = Parse(match.Groups[1], byte.Parse);
        var m = Parse(match.Groups[2], byte.Parse);
        var s = Parse(match.Groups[3], byte.Parse);
        var f = Parse(match.Groups[4], byte.Parse);

        var index = new CueSheetTrackIndex(i, new Msf(m, s, f));

        if (track.Indices.Count == 0)
        {
            var zero = Msf.Zero;

            if (index is not { Number: 0 or 1 } && index.Position != zero && track.Index is 1)
            {
                var message = $"""Track 1 index isn't 0 or 1 and doesn't start at {zero} for "{input}".""";

                throw new InvalidDataException(message);
            }
        }
        else
        {
            var last = track.Indices.Last();

            if (index.Number != last.Number + 1)
            {
                var message = $"""Track index isn't consecutive to previous' for "{input}".""";

                throw new InvalidDataException(message);
            }
        }

        track.Indices.Add(index);

        return true;
    }

    private static bool PerformerHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(PerformerRegex(), input, out var match))
        {
            return false;
        }

        var performer = match.Groups[1].Value;

        if (track == null)
        {
            ThrowIfNot(sheet, s => s.Performer, null, input);

            sheet.Performer = performer;
        }
        else
        {
            ThrowIfNot(track, s => s.Performer, null, input);

            track.Performer = performer;
        }

        return true;
    }

    private static bool PreGapHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(PreGapRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNull(track, input);

        ThrowIfNot(track, s => s.PreGap, null, input);

        var m = Parse(match.Groups[1], byte.Parse);
        var s = Parse(match.Groups[2], byte.Parse);
        var f = Parse(match.Groups[3], byte.Parse);

        track.PreGap = new Msf(m, s, f);

        return true;
    }

    private static bool RemHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(RemRegex(), input, out var match))
        {
            return false;
        }

        var comment = match.Groups[1].Value;

        sheet.Comments.Add(comment);

        return true;
    }

    private static bool TitleHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(TitleRegex(), input, out var match))
        {
            return false;
        }

        var title = match.Groups[1].Value;

        if (track == null)
        {
            ThrowIfNot(sheet, s => s.Title, null, input);

            sheet.Title = title;
        }
        else
        {
            ThrowIfNot(track, s => s.Title, null, input);

            track.Title = title;
        }

        return true;
    }

    private static bool TrackHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(TrackRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNull(file, input);

        var index = Parse(match.Groups[1], int.Parse);

        if (file.Tracks.Count is not 0) // first can be any index
        {
            var last = file.Tracks.Last();

            if (index != last.Index + 1)
            {
                var message = $"Track indices are not consecutive, current is {index}, previous was {last.Index}.";

                throw new InvalidDataException(message);
            }
        }

        var type = match.Groups[2].Value;

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

        track = new CueSheetTrack(index, mode);

        file.Tracks.Add(track);

        return true;
    }

    private static bool IsrcHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(IsrcRegex(), input, out var match))
        {
            return false;
        }

        ThrowIfNull(track, input);

        ThrowIfNot(track, s => s.Isrc, null, input);

        var isrc = match.Groups[1].Value;

        track.Isrc = isrc;

        return true;
    }

    private static bool CommentHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        var success = TryMatch(CommentRegex(), input, out _);

        return success;
    }

    private delegate bool CueSheetHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track);
}