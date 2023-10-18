using System.Diagnostics.CodeAnalysis;
using System.Text;
using WipeoutInstaller.ISO9660;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

// TODO NetBSD-9.3-i386.iso// TODO bootable
// TODO NetBSD-9.3-mac68k // TODO rr jol
// TODO NetBSD-9.3-macppc // TODO rr jol hfs

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestDisc : UnitTestBase
{
    private static readonly Dictionary<string, Dictionary<int, int>> TestIsoReadingLengths = new()
    {
        {
            @"D:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue", new Dictionary<int, int>
            {
                { 1, 1575 },
                { 2, 27441 },
                { 3, 20759 },
                { 4, 4880 },
                { 5, 11608 },
                { 6, 1861 },
                { 7, 4757 },
                { 8, 4780 },
                { 9, 2764 },
                { 10, 6444 },
                { 11, 7989 },
                { 12, 2535 },
                { 13, 8385 },
                { 14, 9153 },
                { 15, 5246 },
                { 16, 2184 },
                { 17, 4741 },
                { 18, 4788 },
                { 19, 6210 }
            }
        },
        {
            @"D:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue", new Dictionary<int, int>
            {
                { 1, 5657 }
            }
        },
        {
            @"D:\Temp\WipEout (Europe) (v1.1) - Multi.cue", new Dictionary<int, int>
            {
                { 1, 27020 },
                { 2, 23506 },
                { 3, 24009 },
                { 4, 22790 },
                { 5, 23526 },
                { 6, 23622 },
                { 7, 23772 },
                { 8, 24335 },
                { 9, 22919 },
                { 10, 28802 },
                { 11, 21813 },
                { 12, 28128 }
            }
        },
        {
            @"D:\Temp\WipEout (Europe) (v1.1) - Single.cue", new Dictionary<int, int>
            {
                { 1, 27020 },
                { 2, 23506 },
                { 3, 24009 },
                { 4, 22790 },
                { 5, 23526 },
                { 6, 23622 },
                { 7, 23772 },
                { 8, 24335 },
                { 9, 22919 },
                { 10, 28802 },
                { 11, 21813 },
                { 12, 28128 }
            }
        }
    };

    private static readonly Dictionary<string, Dictionary<int, int>> TestIsoReadingPositions = new()
    {
        {
            @"D:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue", new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 1575 },
                { 3, 29016 },
                { 4, 49775 },
                { 5, 54655 },
                { 6, 66263 },
                { 7, 68124 },
                { 8, 72881 },
                { 9, 77661 },
                { 10, 80425 },
                { 11, 86869 },
                { 12, 94858 },
                { 13, 97393 },
                { 14, 105778 },
                { 15, 114931 },
                { 16, 120177 },
                { 17, 122361 },
                { 18, 127102 },
                { 19, 131890 }
            }
        },
        {
            @"D:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue", new Dictionary<int, int>
            {
                { 1, 0 }
            }
        },
        {
            @"D:\Temp\WipEout (Europe) (v1.1) - Multi.cue", new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 27170 },
                { 3, 50826 },
                { 4, 74985 },
                { 5, 97925 },
                { 6, 121601 },
                { 7, 145373 },
                { 8, 169295 },
                { 9, 193780 },
                { 10, 216849 },
                { 11, 245801 },
                { 12, 267764 }
            }
        },
        {
            @"D:\Temp\WipEout (Europe) (v1.1) - Single.cue", new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 27170 },
                { 3, 50826 },
                { 4, 74985 },
                { 5, 97925 },
                { 6, 121601 },
                { 7, 145373 },
                { 8, 169295 },
                { 9, 193780 },
                { 10, 216849 },
                { 11, 245801 },
                { 12, 267764 }
            }
        }
    };

    [TestMethod]
    [DataRow(@"D:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue")]
    [DataRow(@"D:\Temp\NetBSD-9.3-i386.iso")]
    [DataRow(@"D:\Temp\NetBSD-9.3-mac68k.iso")]
    [DataRow(@"D:\Temp\NetBSD-9.3-macppc.iso")]
    [DataRow(@"D:\Temp\Rocky-9.2-aarch64-boot.cue")]
    [DataRow(@"D:\Temp\Rocky-9.2-aarch64-boot.iso")]
    [DataRow(@"D:\Temp\Rocky-9.2-aarch64-minimal.cue")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-aarch64-minimal.iso")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-ppc64le-boot.cue")]
    [DataRow(@"D:\Temp\Rocky-9.2-ppc64le-boot.iso")]
    [DataRow(@"D:\Temp\Rocky-9.2-ppc64le-minimal.cue")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-ppc64le-minimal.iso")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-s390x-boot.cue")]
    [DataRow(@"D:\Temp\Rocky-9.2-s390x-boot.iso")]
    [DataRow(@"D:\Temp\Rocky-9.2-s390x-minimal.cue")] // BUG not seen as DVD?
    [DataRow(@"D:\Temp\Rocky-9.2-s390x-minimal.iso")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-x86_64-boot.cue")]
    [DataRow(@"D:\Temp\Rocky-9.2-x86_64-boot.iso")]
    [DataRow(@"D:\Temp\Rocky-9.2-x86_64-minimal.cue")] // DVD
    [DataRow(@"D:\Temp\Rocky-9.2-x86_64-minimal.iso")] // DVD
    [DataRow(@"D:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue")]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Multi.cue")]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Single.cue")]
    public void TestIsoReading(string path)
    {
        var extension = Path.GetExtension(path);

        using var disc = true switch
        {
            true when string.Equals(extension, ".cue", StringComparison.OrdinalIgnoreCase) => LoadDiscFromCue(path),
            true when string.Equals(extension, ".iso", StringComparison.OrdinalIgnoreCase) => LoadDiscFromIso(path),
            _                                                                              => throw new NotSupportedException(path)
        };

        if (disc.Tracks.First().Length > MSF.Max.ToLBA())
        {
            Assert.Inconclusive("Image too long for a CD, most likely a DVD.");
        }

        WriteLine("Tracks:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            WriteLine(track);
        }

        WriteLine("Tracks positions:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            var actual = track.Position;

            WriteLine(actual);

            if (!TestIsoReadingPositions.ContainsKey(path))
            {
                WriteLine($"File is not in dictionary: {path}");
                continue;
            }

            var expected = TestIsoReadingPositions[path][track.Index];

            Assert.AreEqual(expected, actual, $"Track {track.Index}");
        }

        WriteLine("Tracks lengths:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            var actual = track.Length;

            WriteLine(actual);

            if (!TestIsoReadingLengths.ContainsKey(path))
            {
                WriteLine($"File is not in dictionary: {path}");
                continue;
            }

            var expected = TestIsoReadingLengths[path][track.Index];

            Assert.AreEqual(expected, actual, $"Track {track.Index}");
        }

        var isoFileSystem = IsoFileSystem.Read(disc);

        var value = GetTextTree(isoFileSystem.RootDirectory);

        WriteLine(value);
    }

    private Disc LoadDiscFromCue(string path)
    {
        WriteLine(path);

        var sheet = CueSheetParser.Parse(path);

        WriteLine();

        PrintTableOfContents(sheet);

        WriteLine();

        {
            WriteLine("Searching...");
            WriteLine();

            var file = sheet.Files.FirstOrDefault(s => s.Type is CueSheetFileType.Binary)
                       ?? throw new InvalidOperationException("Failed to find file.");

            WriteLine(file.Name);

            var track = file.Tracks.FirstOrDefault(s => s.Type is CueSheetTrackType.Mode1Raw or CueSheetTrackType.Mode2Raw or CueSheetTrackType.Mode1Cooked)
                        ?? throw new InvalidOperationException("Failed to find track.");

            WriteLine(track.Type);

            var index = track.Indices.FirstOrDefault(s => s.Number is 1)
                        ?? throw new InvalidOperationException("Failed to find index.");

            WriteLine(index.Position);
        }

        {
            var disc = new Disc();

            foreach (var file in sheet.Files)
            {
                foreach (var track in file.Tracks)
                {
                    disc.Tracks.Add(new DiscTrackCueBin(track));
                }
            }

            return disc;
        }
    }

    private Disc LoadDiscFromIso(string path)
    {
        WriteLine(path);

        var stream = File.OpenRead(path);

        var track = new DiscTrackIso(stream);

        var disc = new Disc();

        disc.Tracks.Add(track);

        return disc;
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

            if (entry is not IsoFileSystemEntryDirectory directory)
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