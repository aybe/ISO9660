using System.Security.Cryptography;
using ISO9660.FileSystem;
using ISO9660.Tests.Templates;
using ISO9660.WorkInProgress;

namespace ISO9660.Tests;

[TestClass]
public sealed class UnitTestIsoReadFile : UnitTestIso
{
    public static IEnumerable<object[]> TestIsoReadFileInit()
    {
        var files = TestData.GetJsonTestData<TestDataIsoReadFile>(@"Templates\TestDataIsoReadFile.json");

        foreach (var file in files)
        {
            yield return new object[] { file.Source, file.Target, file.Sha256, file.Cooked };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestIsoReadFileInit), DynamicDataSourceType.Method)]
    public void TestIsoReadFile(string source, string target, string sha256, bool cooked)
    {
        using var disc = LoadDiscFromCue(source);

        var ifs = IsoFileSystem.Read(disc);

        var tryFindFile = ifs.TryFindFile(target, out var result);

        Assert.IsTrue(tryFindFile);

        Assert.IsNotNull(result);

        using var stream = new MemoryStream();

        if (cooked)
        {
            disc.ReadFileUser(result, stream);
        }
        else
        {
            disc.ReadFileRaw(result, stream);
        }

        stream.Position = 0;

        var hashData = SHA256.HashData(stream);

        var hashText = string.Concat(hashData.Select(s => s.ToString("x2")));

        Assert.AreEqual(sha256, hashText, StringComparer.OrdinalIgnoreCase);
    }
}