using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace WipeoutInstaller.WorkInProgress;

public static partial class CueSheetParser
// https://www.gnu.org/software/ccd2cue/manual/html_node/CUE-sheet-format.html#CUE-sheet-format
// https://web.archive.org/web/20070614044112/http://www.goldenhawk.com/download/cdrwin.pdf
// https://github.com/libyal/libodraw/blob/main/documentation/CUE%20sheet%20format.asciidoc
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

        if (sheet.Catalog != null)
        {
            throw new InvalidDataException();
        }

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

        if (file != null)
        {
            sheet.Files.Add(file);
        }

        var name = match.Groups[1].Value;
        var type = match.Groups[2].Value;

        var fileType = type switch
        {
            "BINARY"   => CueSheetFileType.Binary,
            "MOTOROLA" => CueSheetFileType.Motorola,
            "AUDIO"    => CueSheetFileType.Audio,
            "AIFF"     => CueSheetFileType.Aiff,
            "FLAC"     => CueSheetFileType.Flac,
            "MP3"      => CueSheetFileType.Mp3,
            "WAVE"     => CueSheetFileType.Wave,
            _          => throw new InvalidOperationException()
        };

        file = new CueSheetFile(name, fileType);

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

        if (track.Flags != CueSheetTrackFlags.None)
        {
            throw new InvalidDataException();
        }

        foreach (var capture in match.Groups[1].Captures.Cast<Capture>())
        {
            var value = capture.Value;

            var flags = value switch
            {
                "4CH"  => CueSheetTrackFlags.FourChannelAudio,
                "PRE"  => CueSheetTrackFlags.PreEmphasis,
                "SCMS" => CueSheetTrackFlags.SerialCopyManagementSystem,
                "DCP"  => CueSheetTrackFlags.DigitalCopyPermitted,
                _      => throw new InvalidOperationException()
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

        var i = Parse(match.Groups[1], int.Parse);
        var m = Parse(match.Groups[2], int.Parse);
        var s = Parse(match.Groups[3], int.Parse);
        var f = Parse(match.Groups[4], int.Parse);

        var index = new CueSheetTrackIndex(i, m, s, f);

        track.Indices.Add(index); // TODO check that these are contiguous?

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

        if (track != null)
        {
            if (track.Performer != null)
            {
                throw new InvalidDataException();
            }

            track.Performer = performer;

            return true;
        }

        if (sheet.Performer != null)
        {
            throw new InvalidDataException();
        }

        sheet.Performer = performer;

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

        if (track.PreGap != null)
        {
            throw new InvalidDataException();
        }

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

        if (track != null)
        {
            if (track.Title != null)
            {
                throw new InvalidDataException();
            }

            track.Title = title;

            return true;
        }

        if (sheet.Title != null)
        {
            throw new InvalidDataException();
        }

        sheet.Title = title;

        return true;
    }

    private static bool TrackHandler(
        string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        // TODO at the end of parsing, there must be at least one track

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

        if (track.Isrc != null)
        {
            throw new InvalidDataException();
        }

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