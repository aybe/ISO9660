using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace WipeoutInstaller.WorkInProgress;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static partial class CueSheetParser
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

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var match1 = FileRegex().Match(line);
            if (match1.Success)
            {
                if (file != null)
                {
                    sheet.Files.Add(file);
                }

                var name = match1.Groups[1].Value;
                var type = match1.Groups[2].Value;
                file = new CueSheetFile(name, type);
                continue;
            }

            var match2 = TrackRegex().Match(line);
            if (match2.Success)
            {
                var groups = match2.Groups;
                var index = int.Parse(groups[1].Value);
                var type = groups[2].Value;

                track = new CueSheetTrack(index, type);

                if (file is null)
                {
                    throw new InvalidOperationException();
                }

                file.Tracks.Add(track);
                continue;
            }

            var match = FlagsRegex().Match(line);
            if (match.Success)
            {
                if (track == null)
                {
                    throw new InvalidOperationException();
                }

                if (track.Flags.Count > 0)
                {
                    throw new InvalidDataException();
                }

                foreach (Capture c in match.Groups[1].Captures) // todo
                {
                    track.Flags.Add(c.Value);
                }

                continue;
            }

            var match3 = IndexRegex().Match(line);
            if (match3.Success)
            {
                var groups = match3.Groups;
                var number = int.Parse(groups[1].Value);
                var m = int.Parse(groups[2].Value);
                var s = int.Parse(groups[3].Value);
                var f = int.Parse(groups[4].Value);
                if (track is null)
                {
                    throw new InvalidOperationException();
                }

                var index = new CueSheetTrackIndex(number, m, s, f);

                // todo check contiguous?
                track.Indices.Add(index);
                continue;
            }

            var match4 = RemRegex().Match(line);
            if (match4.Success)
            {
                sheet.Comments.Add(match4.Groups[1].Value);
                continue;
            }

            var match5 = CatalogRegex().Match(line);
            if (match5.Success)
            {
                if (sheet.Catalog != null)
                {
                    throw new InvalidDataException();
                }

                sheet.Catalog = ulong.Parse(match5.Groups[1].Value);
                continue;
            }

            var titleRegex = TitleRegex().Match(line);
            if (titleRegex.Success)
            {
                var value = titleRegex.Groups[1].Value;

                if (track != null)
                {
                    if (track.Title != null)
                    {
                        throw new InvalidDataException();
                    }

                    track.Title = value;
                    continue;
                }

                if (sheet.Title != null)
                {
                    throw new InvalidDataException();
                }

                sheet.Title = value;
                continue;
            }

            var performerRegex = PerformerRegex().Match(line);
            if (performerRegex.Success)
            {
                var performer = performerRegex.Groups[1].Value;
                if (track != null)
                {
                    if (track.Performer != null)
                    {
                        throw new InvalidDataException();
                    }

                    track.Performer = performer;
                    continue;
                }

                if (sheet.Performer != null)
                {
                    throw new InvalidDataException();
                }

                sheet.Performer = performer;
                continue;
            }

            var preGapRegex = PreGapRegex().Match(line);
            if (preGapRegex.Success)
            {
                var m = byte.Parse(preGapRegex.Groups[1].Value);
                var s = byte.Parse(preGapRegex.Groups[2].Value);
                var f = byte.Parse(preGapRegex.Groups[3].Value);
                var msf = new Msf(m, s, f);
                if (track == null)
                {
                    throw new InvalidOperationException();
                }

                if (track.PreGap != null)
                {
                    throw new InvalidDataException();
                }

                track.PreGap = msf;
                continue;
            }

            var isrcRegex = IsrcRegex().Match(line);
            if (isrcRegex.Success)
            {
                if (track == null)
                {
                    throw new InvalidOperationException();
                }

                if (track.Isrc != null)
                {
                    throw new InvalidDataException();
                }

                track.Isrc = isrcRegex.Groups[1].Value;
                continue;
            }

            if (line.StartsWith(';'))
            {
                continue; // non-compliant
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
}