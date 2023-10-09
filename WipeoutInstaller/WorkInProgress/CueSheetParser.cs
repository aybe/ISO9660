using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static partial class CueSheetParser
{
    private static readonly IReadOnlyDictionary<Regex, CueSheetHandler> Handlers =
        new Dictionary<Regex, CueSheetHandler>
            {
                { FileRegex(), FileHandler },
                { CatalogRegex(), CatalogHandler },
                { FlagsRegex(), FlagsHandler },
                { IndexRegex(), IndexHandler },
                { PerformerRegex(), PerformerHandler },
                { PreGapRegex(), PreGapHandler },
                { RemRegex(), RemHandler },
                { TitleRegex(), TitleHandler },
                { TrackRegex(), TrackHandler },
                { IsrcRegex(), IsrcHandler },
                { CommentRegex(), CommentHandler }
            }
            .AsReadOnly();

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

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var handled = false;

            foreach (var (regex, handler) in Handlers)
            {
                if (!handler(regex, line, sheet, ref file, ref track))
                {
                    continue;
                }

                handled = true;
                break;
            }

            if (handled)
            {
                continue;
            }

            throw new NotSupportedException(line);
        }

        return sheet;
    }

    [GeneratedRegex("""^\s*CATALOG\s+(\d{13})\s*\r?$""")]
    private static partial Regex CatalogRegex();

    [GeneratedRegex("""^\s*FILE\s+"?(.*)"?\s+(BINARY|MOTORLA|AUDIO|AIFF|FLAC|MP3|WAVE)\s*\r?$""")]
    private static partial Regex FileRegex();

    [GeneratedRegex("""^\s*FLAGS(?:\s+(4CH|PRE|SCMS|DCP))+\s*\r?$""")]
    private static partial Regex FlagsRegex();

    [GeneratedRegex("""^\s*INDEX\s+(0?[0-9]|[1-9][0-9])\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""")]
    private static partial Regex IndexRegex();

    [GeneratedRegex("""^\s*PERFORMER\s+"?(.*)"?\s*\r?$""")]
    private static partial Regex PerformerRegex();

    [GeneratedRegex("""^\s*PREGAP\s+(0?[0-9]|[1-9][0-9]):([0-5][0-9]):([0-6][0-9]|7[0-4])\s*\r?$""")]
    private static partial Regex PreGapRegex();

    [GeneratedRegex("""^\s*REM\s+(.*)\s*\r?$""")]
    private static partial Regex RemRegex();

    [GeneratedRegex("""^\s*TITLE\s+"?(.*)"?\s*\r?$""")]
    private static partial Regex TitleRegex();

    [GeneratedRegex("""^\s*TRACK\s+(0?[0-9]|[1-9][0-9])\s+(AUDIO|CDG|MODE1/2048|MODE1/2352|MODE2/2048|MODE2/2324|MODE2/2336|MODE2/2352|CDI/2336|CDI/2352)\s*\r?$""")]
    private static partial Regex TrackRegex();

    [GeneratedRegex("""^\s*ISRC\s+(\w{5}\d{7})\s*\r?$""")]
    private static partial Regex IsrcRegex();

    [GeneratedRegex("""^\s*;.*\r?$""")]
    private static partial Regex CommentRegex();

    private static bool TryMatch(in Regex regex, in string input, out Match match)
    {
        match = regex.Match(input);

        var success = match.Success;

        return success;
    }

    private static bool CatalogHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (sheet.Catalog != null)
        {
            throw new InvalidDataException();
        }

        var catalog = ulong.Parse(match.Groups[1].Value);

        sheet.Catalog = catalog;

        return true;
    }

    private static bool FileHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (file != null)
        {
            sheet.Files.Add(file);
        }

        var name = match.Groups[1].Value;
        var type = match.Groups[2].Value;

        file = new CueSheetFile(name, type);

        return true;
    }

    private static bool FlagsHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (track is null)
        {
            throw new InvalidOperationException();
        }

        if (track.Flags.Count > 0)
        {
            throw new InvalidDataException();
        }

        foreach (var capture in match.Groups[1].Captures.Cast<Capture>()) // TODO
        {
            track.Flags.Add(capture.Value);
        }

        return true;
    }

    private static bool IndexHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (track is null)
        {
            throw new InvalidOperationException();
        }

        var i = int.Parse(match.Groups[1].Value);
        var m = int.Parse(match.Groups[2].Value);
        var s = int.Parse(match.Groups[3].Value);
        var f = int.Parse(match.Groups[4].Value);

        var index = new CueSheetTrackIndex(i, m, s, f);

        track.Indices.Add(index); // TODO check that these are contiguous?

        return true;
    }

    private static bool PerformerHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
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
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (track == null)
        {
            throw new InvalidOperationException();
        }

        if (track.PreGap != null)
        {
            throw new InvalidDataException();
        }

        var m = byte.Parse(match.Groups[1].Value);
        var s = byte.Parse(match.Groups[2].Value);
        var f = byte.Parse(match.Groups[3].Value);

        track.PreGap = new Msf(m, s, f);

        return true;
    }

    private static bool RemHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        var comment = match.Groups[1].Value;

        sheet.Comments.Add(comment);

        return true;
    }

    private static bool TitleHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
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
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (file is null)
        {
            throw new InvalidOperationException();
        }

        var index = int.Parse(match.Groups[1].Value);
        var type = match.Groups[2].Value; // TODO

        track = new CueSheetTrack(index, type);

        file.Tracks.Add(track);

        return true;
    }

    private static bool IsrcHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        if (!TryMatch(regex, input, out var match))
        {
            return false;
        }

        if (track == null)
        {
            throw new InvalidOperationException();
        }

        if (track.Isrc != null)
        {
            throw new InvalidDataException();
        }

        var isrc = match.Groups[1].Value;

        track.Isrc = isrc;

        return true;
    }

    private static bool CommentHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track)
    {
        var success = TryMatch(regex, input, out _);

        return success;
    }

    private delegate bool CueSheetHandler(
        Regex regex, string input, CueSheet sheet, ref CueSheetFile? file, ref CueSheetTrack? track);
}