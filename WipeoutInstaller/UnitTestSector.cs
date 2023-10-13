using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestSector : UnitTestBase
{

    [TestMethod]
    public void TestSectorSizeAudio()
    {
        TestSectorSize<SectorAudio>();
    }

    [TestMethod]
    public void TestSectorSizeMode0()
    {
        TestSectorSize<SectorMode0>();
    }

    [TestMethod]
    public void TestSectorSizeMode1()
    {
        TestSectorSize<SectorMode1>();
    }

    [TestMethod]
    public void TestSectorSizeMode2Form1()
    {
        TestSectorSize<SectorMode2Form1>();
    }

    [TestMethod]
    public void TestSectorSizeMode2Form2()
    {
        TestSectorSize<SectorMode2Form2>();
    }

    [TestMethod]
    public void TestSectorSizeMode2FormLess()
    {
        TestSectorSize<SectorMode2FormLess>();
    }

    private static void TestSectorSize<T>() where T : struct, ISector
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<T>());
    }

    [TestMethod]
    public void TestSizeOf()
    {
        Assert.AreEqual(3, Unsafe.SizeOf<SectorAddress>());
        Assert.AreEqual(4, Unsafe.SizeOf<SectorSubHeader>());
        Assert.AreEqual(8, Unsafe.SizeOf<SectorSubHeaderBlock>());
    }
}