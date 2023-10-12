// ReSharper disable StringLiteralTypo

using JetBrains.Annotations;
using WipeoutInstaller.WorkInProgress;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WipeoutInstaller;

[TestClass]
public class UnitTestCueSheet
{
    [PublicAPI]
    public required TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.cue")]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).cue")]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1) - Multi.cue")]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1) - Single.cue")]
    public void TestParsing(string path)
    {
        using var stream = File.OpenRead(path);

        ParseCueSheet(stream, TestContext);
    }

    public static void ParseCueSheet(Stream stream, TestContext context)
    {
        var sheet = CueSheetParser.Parse(stream);

        foreach (var file in sheet.Files)
        {
            context.WriteLine($"File: {file}");

            foreach (var track in file.Tracks)
            {
                context.WriteLine($"\tTrack: {track}, PreGap: {track.PreGap}");

                foreach (var index in track.Indices)
                {
                    context.WriteLine($"\t\tIndex: {index}, LBA = {index.Position.ToLBA().Position:N0}, Byte = {index.Position.ToLBA().Position * 2352:N0}");
                }
            }
        }
    }
}