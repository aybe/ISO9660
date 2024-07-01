using System.Text;
using ISO9660.Logical;
using ISO9660.Physical;

namespace ISO9660.Tests;

[TestClass]
public sealed class UnitTestRealDevice : UnitTestBase
{
    [TestMethod]
    public void TestMethod1()
    {
        var info = GetDrive();

        using var disc = Disc.Open(info.Name);

        foreach (var track in disc.Tracks)
        {
            WriteLine(track);
        }

        var fs = IsoFileSystem.Read(disc);

        var builder = new StringBuilder();

        fs.Print(builder);

        WriteLine(builder.ToString());
    }

    private static DriveInfo GetDrive()
    {
        var info = DriveInfo.GetDrives().FirstOrDefault(s => s is { DriveType: DriveType.CDRom, IsReady: true });

        return info ?? throw new InvalidOperationException();
    }
}