using System.Runtime.InteropServices;

namespace WipeoutInstaller.WorkInProgress;

[TestClass]
public class SectorTest
{
    [TestMethod]
    public void TestMode1()
    {
        Assert.AreEqual(2352, Marshal.SizeOf<SectorAudio>());
        Assert.AreEqual(2352, Marshal.SizeOf<SectorMode0>());
        Assert.AreEqual(2352, Marshal.SizeOf<SectorMode1>());
        Assert.AreEqual(2352, Marshal.SizeOf<SectorMode2Formless>());
        Assert.AreEqual(2352, Marshal.SizeOf<SectorMode2Form1>());
        Assert.AreEqual(2352, Marshal.SizeOf<SectorMode2Form2>());
    }
}