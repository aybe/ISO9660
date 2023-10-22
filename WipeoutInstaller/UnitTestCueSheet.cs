// ReSharper disable StringLiteralTypo

using ISO9660.CDRWIN;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.WorkInProgress;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WipeoutInstaller;

[TestClass]
public class UnitTestCueSheet : UnitTestBase
{
    [TestMethod]
    [DataRow(@"D:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue")]
    [DataRow(@"D:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue")]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Multi.cue")]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Single.cue")]
    public void TestParsing(string path)
    {
        ParseCueSheet(path);
    }

    [TestMethod]
    [DataRow(@"D:\Temp\CueFileList.txt")]
    public void TestParsingList(string path)
    {
        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                if (File.Exists(line))
                {
                    TestContext.WriteLine();
                    TestContext.WriteLine($"Trying to parse file: {line}");
                    TestContext.WriteLine();

                    ParseCueSheet(line);
                }
                else
                {
                    TestContext.WriteLine($"File not found: {line}");
                }
            }
        }
        else
        {
            TestContext.WriteLine($"File list not found: {path}");
        }
    }

    private void ParseCueSheet(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        foreach (var file in sheet.Files)
        {
            using var indent1 = Indent(1);

            WriteLine($"File: {file}");

            foreach (var track in file.Tracks)
            {
                using var indent2 = Indent(2);

                WriteLine($"Track: {track}");

                foreach (var index in track.Indices)
                {
                    using var indent3 = Indent(3);

                    WriteLine($"Index: {index}");
                }
            }
        }
    }
}