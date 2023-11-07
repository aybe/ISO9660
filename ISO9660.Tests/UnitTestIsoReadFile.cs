using System.Security.Cryptography;
using ISO9660.Logical;
using ISO9660.Physical;
using ISO9660.Tests.Templates;

namespace ISO9660.Tests;

[TestClass]
public sealed class UnitTestIsoReadFile : UnitTestBase
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
    public async Task TestIsoReadFile(string source, string target, string sha256, bool cooked)
    {
        await using var disc = Disc.FromCue(source);

        var ifs = IsoFileSystem.Read(disc);

        var tryFindFile = ifs.TryFindFile(target, out var result);

        Assert.IsTrue(tryFindFile);

        Assert.IsNotNull(result);

        using var stream = new MemoryStream();

        if (cooked)
        {
            await disc.ReadFileUserAsync(result, stream);
        }
        else
        {
            await disc.ReadFileRawAsync(result, stream);
        }

        stream.Position = 0;

        var hashData = await SHA256.HashDataAsync(stream);

        var hashText = string.Concat(hashData.Select(s => s.ToString("x2")));

        Assert.AreEqual(sha256, hashText, StringComparer.OrdinalIgnoreCase);
    }
}