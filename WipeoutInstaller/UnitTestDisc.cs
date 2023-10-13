using System.Diagnostics.CodeAnalysis;
using System.Text;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestDisc : UnitTestBase
{
    [TestMethod]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue")]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue")]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1) - Multi.cue")]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1) - Single.cue")]
    public void TestIsoReading(string path)
    {
        using var stream = File.OpenRead(path);

        var sheet = CueSheetParser.Parse(stream);

        WriteLine();

        PrintTableOfContents(sheet);

        WriteLine();

        using var disc = LoadFileSystem(sheet, path);

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            WriteLine(track);
        }

        if (!disc.TryGetIso9660FileSystem(out var result))
        {
            throw new InvalidOperationException("The disc has no ISO9660 file system.");
        }

        var rootDirectory = result.GetRootDirectory();

        var value = GetTextTree(rootDirectory);

        WriteLine(value);
    }

    private static string GetTextTree(IsoFileSystemEntry rootDirectory)
    {
        var builder = new StringBuilder();

        var stack = new Stack<(IsoFileSystemEntry Entry, int Depth)>();

        stack.Push((rootDirectory, 0));

        while (stack.Count > 0)
        {
            var (entry, i) = stack.Pop();

            builder.AppendLine($"{new string('\t', i)}{entry.Name}");

            if (entry is not IsoDirectory directory)
            {
                continue;
            }

            foreach (var item in directory.Directories.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }

            foreach (var item in directory.Files.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }
        }

        var tree = builder.ToString();

        return tree;
    }

    private Disc LoadFileSystem(CueSheet sheet, string path) // TODO cue sheet could hold path
    {
        var fileDirectory = Path.GetDirectoryName(path)
                            ?? throw new InvalidOperationException("Failed to get directory name.");

        {
            WriteLine("Searching...");
            WriteLine();

            var file = sheet.Files.FirstOrDefault(s => s.Type is CueSheetFileType.Binary)
                       ?? throw new InvalidOperationException("Failed to find file.");

            WriteLine(file.Name);

            var track = file.Tracks.FirstOrDefault(s => s.Type is CueSheetTrackType.Mode1Raw or CueSheetTrackType.Mode2Raw)
                        ?? throw new InvalidOperationException("Failed to find track.");

            WriteLine(track.Type);

            var index = track.Indices.FirstOrDefault(s => s.Number is 1)
                        ?? throw new InvalidOperationException("Failed to find index.");

            WriteLine(index.Position);


            var filePath = Path.Combine(fileDirectory, file.Name);

            using var fileStream = File.OpenRead(filePath);

            var fileSectorMode = GetSectorType(track.Type);

            WriteLine(fileSectorMode);
        }

        {
            var disc = new Disc();

            foreach (var file in sheet.Files)
            {
                foreach (var track in file.Tracks)
                {
                    disc.Tracks.Add(new DiscTrackCueBin(fileDirectory, file, track));
                }
            }

            return disc;
        }
    }

    private static SectorType GetSectorType(CueSheetTrackType trackType)
    {
        return trackType switch
        {
            CueSheetTrackType.Mode1Raw => SectorType.Mode1,
            CueSheetTrackType.Mode2Raw => SectorType.Mode2Form1,
            _                          => throw new ArgumentOutOfRangeException(nameof(trackType), trackType, null)
        };
    }

    private void PrintTableOfContents(CueSheet sheet)
    {
        WriteLine("Table of contents:");
        WriteLine();

        foreach (var file in sheet.Files)
        {
            using var indent1 = Indent(1);

            WriteLine(file);

            foreach (var track in file.Tracks)
            {
                using var indent2 = Indent(2);

                WriteLine(track);
            }
        }
    }
}