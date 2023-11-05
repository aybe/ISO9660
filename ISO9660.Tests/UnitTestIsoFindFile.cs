using ISO9660.Logical;
using ISO9660.Physical;
using ISO9660.Tests.Templates;

namespace ISO9660.Tests;

[TestClass]
public sealed class UnitTestIsoFindFile : UnitTestBase
{
    public static IEnumerable<object[]> TestIsoFindFileInit()
    {
        var files = TestData.GetJsonTestData<TestDataIsoFindFile>(@"Templates\TestDataIsoFindFile.json");

        foreach (var file in files)
        {
            yield return new object[] { file.Source, file.Target, file.Exists };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestIsoFindFileInit), DynamicDataSourceType.Method)]
    public void TestIsoFindFile(string source, string target, bool exists)
    {
        using var disc = Disc.FromCue(source);

        var ifs = IsoFileSystem.Read(disc);

        var tryFindFile = ifs.TryFindFile(target, out var result);

        Assert.AreEqual(exists, tryFindFile);

        if (result == null)
        {
            return;
        }

        WriteLine(() => result);
    }
}