// ReSharper disable StringLiteralTypo

using JetBrains.Annotations;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

[TestClass]
public class UnitTestIso9660
{
    [PublicAPI]
    public required TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow(@"C:\Temp\Wipeout\WipEout (Europe) (v1.1)\WipEout (Europe) (v1.1) (Track 01).bin")]
    public void TestIsoReading(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        using var fs = new IsoImage(stream);

        fs.Logger = TestContext.WriteLine;

        var rootDirectory = fs.GetRootDirectory();

        var entries = new Stack<IsoFileSystemEntry>();

        entries.Push(rootDirectory);

        while (entries.Count > 0)
        {
            var entry = entries.Pop();

            if (entry is IsoDirectory directory)
            {
                foreach (var directoryDirectory in directory.Directories.AsEnumerable().Reverse())
                {
                    entries.Push(directoryDirectory);
                }

                foreach (var directoryFile in directory.Files)
                {
                    entries.Push(directoryFile);
                }
            }

            TestContext.WriteLine(entry);
        }
    }
}