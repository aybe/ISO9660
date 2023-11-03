using ISO9660.CDRWIN;
using ISO9660.Tests.Templates;

namespace ISO9660.Tests;

[TestClass]
public class UnitTestCueSheet : UnitTestBase
{
    public static IEnumerable<object[]> TestCueSheetListInit()
    {
        var files = TestData.GetJsonTestData<TestDataCueSheet>(@"Templates\TestDataCueSheet.json");

        foreach (var file in files)
        {
            yield return new object[] { file.Path };
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestCueSheetListInit), DynamicDataSourceType.Method)]
    public void TestCueSheet(string path)
    {
        if (File.Exists(path))
        {
            ParseCueSheet(path);
        }
        else
        {
            Assert.Inconclusive($"File not found: {path}");
        }
    }

    private void ParseCueSheet(string path)
    {
        var sheet = CueSheetParser.Parse(path);

        foreach (var file in sheet.Files)
        {
            using var indent1 = Indent(1);

            WriteLine($"File: {file}");

            foreach (var track in file.Tracks)
            {
                using var indent2 = Indent(2);

                WriteLine($"Track: {track}");

                foreach (var index in track.Indices)
                {
                    using var indent3 = Indent(3);

                    WriteLine($"Index: {index}");
                }
            }
        }
    }
}