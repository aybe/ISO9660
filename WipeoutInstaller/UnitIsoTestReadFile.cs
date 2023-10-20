using System.Security.Cryptography;
using WipeoutInstaller.ISO9660;
using WipeoutInstaller.Templates;

namespace WipeoutInstaller;

[TestClass]
public sealed class UnitIsoTestReadFile : UnitTestDisc
{
    public static IEnumerable<object[]> TestIsoReadFileInit()
    {
        var files = TestData.GetCsvTestData<TestDataIsoReadFile>(@"Templates\TestDataIsoReadFile.csv");

        foreach (var file in files)
        {
            yield return new object[] { file.Source, file.Target, file.Sha256, file.Mode };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestIsoReadFileInit), DynamicDataSourceType.Method)]
    public void TestIsoReadFile(string source, string target, string sha256, DiscReadFileMode mode)
    {
        using var disc = LoadDiscFromCue(source);

        var ifs = IsoFileSystem.Read(disc);

        var tryFindFile = ifs.TryFindFile(target, out var result);

        Assert.IsTrue(tryFindFile);

        Assert.IsNotNull(result);

        using var stream = new MemoryStream();

        disc.ReadFile(result, mode, stream);

        stream.Position = 0;

        var hashData = SHA256.HashData(stream);

        var hashText = string.Concat(hashData.Select(s => s.ToString("x2")));

        Assert.AreEqual(sha256, hashText, StringComparer.OrdinalIgnoreCase);
    }
}