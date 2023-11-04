using System.Diagnostics.CodeAnalysis;
using System.Text;
using ISO9660.FileSystem;
using ISO9660.Media;
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
    public void TestIsoReadFileSystem(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        using var disc = extension switch
        {
            ".cue" => Disc.FromCue(path),
            ".iso" => Disc.FromIso(path),
            _      => throw new NotSupportedException(path)
        };

        var fs = IsoFileSystem.Read(disc);

        var builder = new StringBuilder();

        var stack = new Stack<(IsoFileSystemEntry Entry, int Depth)>();

        stack.Push((fs.RootDirectory, 0));

        while (stack.Count > 0)
        {
            var (entry, i) = stack.Pop();

            builder.AppendLine($"{new string('\t', i)}{entry.FileName}");

            if (entry is not IsoFileSystemEntryDirectory directory)
            {
                continue;
            }

            foreach (var item in directory.Directories.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }

            foreach (var item in directory.Files.AsEnumerable().Reverse())
            {
                stack.Push((item, i + 1));
            }
        }

        WriteLine(builder.ToString());
    }
}