using System.Runtime.CompilerServices;
using WipeoutInstaller.XA;

namespace WipeoutInstaller;

[TestClass]
public class UnitTestSector
{
    [TestMethod]
    public void TestSizeOf()
    {
        Assert.AreEqual(4, Unsafe.SizeOf<SectorSubHeader>());
        Assert.AreEqual(8, Unsafe.SizeOf<SectorSubHeaderBlock>());
        Assert.AreEqual(3, Unsafe.SizeOf<SectorAddress>());
        Assert.AreEqual(SectorMode2.Size, Unsafe.SizeOf<SectorMode2>());
    }
}