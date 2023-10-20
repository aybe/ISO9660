using System.Diagnostics.CodeAnalysis;
using WipeoutInstaller.ISO9660;
using WipeoutInstaller.Templates;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public sealed class UnitTestIsoReadFileSystem : UnitTestIso
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

    public static IEnumerable<object[]> TestIsoReadFileInit()
    {
        var files = TestData.GetCsvTestData<TestDataIsoReadFileSystem>(@"Templates\TestDataIsoReadFileSystem.csv");

        foreach (var file in files)
        {
            yield return new object[] { file.Path };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestIsoReadFileInit), DynamicDataSourceType.Method)]
    public void TestIsoReadFileSystem(string path)
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
}