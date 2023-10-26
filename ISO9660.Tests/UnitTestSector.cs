using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ISO9660.Tests.WorkInProgress;

namespace ISO9660.Tests;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestSector : UnitTestBase
{
    [TestMethod]
    public void TestSizeOf()
    {
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorAudio>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode0>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode1>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2Form1>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2Form2>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2FormLess>());
    }
}