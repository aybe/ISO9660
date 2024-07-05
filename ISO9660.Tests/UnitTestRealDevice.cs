using System.Security.Cryptography;
using System.Text;
using ISO9660.Logical;
using ISO9660.Physical;

namespace ISO9660.Tests;

[TestClass]
public sealed class UnitTestRealDevice : UnitTestBase
{
    [TestMethod]
    public async Task TestMethod1() 
    {
        var info = GetDrive();

        await using var disc = Disc.Open(info.Name);

        foreach (var track in disc.Tracks)
        {
            WriteLine(track);
        }

        var fs = IsoFileSystem.Read(disc);

        if (fs.TryFindFile("/SCES_000.10", out var file1))
        {
            using var stream = new MemoryStream();
            await disc.ReadFileUserAsync(file1, stream);
            stream.Position = 0;
            var hash = await SHA1.HashDataAsync(stream);
            const string expected = "5ca9383a1b988baef91fe4ca686855dc0e70db74";
            var actual = string.Concat(hash.Select(s => s.ToString("x2")));
            Assert.AreEqual(expected, actual, true);
        }

        if (fs.TryFindFile("/WOPAL.AV", out var file2))
        {
            using var stream = new MemoryStream();
            await disc.ReadFileRawAsync(file2, stream);
            stream.Position = 0;
            var hash = await SHA1.HashDataAsync(stream);
            const string expected = "ea236c70fe7c704a62fbe88f46c2e3e12acdf8c9";
            var actual = string.Concat(hash.Select(s => s.ToString("x2")));
            Assert.AreEqual(expected, actual, true);
        }

        var builder = new StringBuilder();

        fs.Print(builder);

        WriteLine(builder.ToString());
    }

    private static DriveInfo GetDrive()
    {
        var info = DriveInfo.GetDrives().FirstOrDefault(s => s is { DriveType: DriveType.CDRom, IsReady: true });

        return info ?? throw new InvalidOperationException("Failed to find a CD-ROM drive.");
    }
}