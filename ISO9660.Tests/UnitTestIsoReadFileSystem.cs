using System.Diagnostics.CodeAnalysis;
using System.Text;
using ISO9660.Logical;
using ISO9660.Physical;
using ISO9660.Tests.Templates;

namespace ISO9660.Tests;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public sealed class UnitTestIsoReadFileSystem : UnitTestBase
{
    public static IEnumerable<object[]> TestIsoReadFileInit()
    {
        var files = TestData.GetJsonTestData<TestDataIsoReadFileSystem>(@"Templates\TestDataIsoReadFileSystem.json");

        foreach (var file in files)
        {
            yield return new object[] { file.Path };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestIsoReadFileInit), DynamicDataSourceType.Method)]
    public async Task TestIsoReadFileSystem(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        await using var disc = extension switch
        {
            ".cue" => Disc.Open(path),
            ".iso" => Disc.Open(path),
            _      => throw new NotSupportedException(path)
        };

        var fs = IsoFileSystem.Read(disc);

        var builder = new StringBuilder();

        fs.Print(builder);

        WriteLine(builder.ToString());
    }
}