using System.Diagnostics.CodeAnalysis;
using ISO9660.FileSystem;
using ISO9660.Tests.Templates;

namespace ISO9660.Tests;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public sealed class UnitTestIsoReadFileSystem : UnitTestIso
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
    public void TestIsoReadFileSystem(string path)
    {
        var extension = Path.GetExtension(path);

        using var disc = true switch
        {
            true when string.Equals(extension, ".cue", StringComparison.OrdinalIgnoreCase) => LoadDiscFromCue(path),
            true when string.Equals(extension, ".iso", StringComparison.OrdinalIgnoreCase) => LoadDiscFromIso(path),
            _                                                                              => throw new NotSupportedException(path)
        };

        WriteLine("Tracks:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            WriteLine(track);
        }

        WriteLine("Tracks positions:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            var actual = track.Position;

            WriteLine(actual);
        }

        WriteLine("Tracks lengths:");

        foreach (var track in disc.Tracks)
        {
            using var indent1 = Indent(1);

            var actual = track.Length;

            WriteLine(actual);
        }

        var isoFileSystem = IsoFileSystem.Read(disc);

        var value = GetTextTree(isoFileSystem.RootDirectory);

        WriteLine(value);
    }
}