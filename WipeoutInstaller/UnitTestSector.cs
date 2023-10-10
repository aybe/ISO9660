using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.WorkInProgress;
using WipeoutInstaller.XA;
using SectorMode = WipeoutInstaller.WorkInProgress.SectorMode;

namespace WipeoutInstaller;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestSector
{
    [PublicAPI]
    public required TestContext TestContext { get; set; }

    [TestMethod]
    public void TestSectorSizeAudio()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorAudio>());
    }

    [TestMethod]
    public void TestSectorSizeMode0()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorMode0>());
    }

    [TestMethod]
    public void TestSectorSizeMode1()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorMode1>());
    }

    [TestMethod]
    public void TestSectorSizeMode2Form1()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorMode2Form1>());
    }

    [TestMethod]
    public void TestSectorSizeMode2Form2()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorMode2Form2>());
    }

    [TestMethod]
    public void TestSectorSizeMode2FormLess()
    {
        Assert.AreEqual(ISector.Size, Unsafe.SizeOf<SectorMode2Formless>());
    }

    [TestMethod]
    public void TestSizeOf()
    {
        Assert.AreEqual(4, Unsafe.SizeOf<SectorSubHeader>());
        Assert.AreEqual(8, Unsafe.SizeOf<SectorSubHeaderBlock>());
        Assert.AreEqual(3, Unsafe.SizeOf<SectorAddress>());
        Assert.AreEqual(SectorConstants.Size, Unsafe.SizeOf<SectorMode2>());
    }

    [TestMethod]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).bin", 16, SectorMode.Mode1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 16, SectorMode.Mode2Form1)]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.bin", 1605, SectorMode.Mode2Form2)]
    public void TestSectorModeUsingEdc(string path, int sectorIndex, SectorMode sectorMode)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        Assert.AreEqual(0, stream.Length % SectorConstants.Size);

        stream.Position = sectorIndex * SectorConstants.Size;

        var bytes = reader.ReadBytes(SectorConstants.Size);

        var edc = sectorMode switch
        {
            SectorMode.Audio         => throw new NotImplementedException(),
            SectorMode.Mode0         => throw new NotImplementedException(),
            SectorMode.Mode1         => CheckEdc<SectorMode1>(bytes),
            SectorMode.Mode2Form1    => CheckEdc<SectorMode2Form1>(bytes),
            SectorMode.Mode2Form2    => CheckEdc<SectorMode2Form2>(bytes),
            SectorMode.Mode2FormLess => throw new NotImplementedException(),
            _                        => throw new ArgumentOutOfRangeException(nameof(sectorMode), sectorMode, null)
        };

        Assert.IsTrue(edc);
    }

    [TestMethod]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).bin", 16, SectorMode.Mode1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 16, SectorMode.Mode2Form1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 27170, SectorMode.Audio)]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.bin", 1605, SectorMode.Mode2Form2)]
    public void TestSectorModeUsingInspection(string path, int sectorIndex, SectorMode sectorModeExpected)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        Assert.AreEqual(0, stream.Length % SectorConstants.Size);

        stream.Position = sectorIndex * SectorConstants.Size;

        var bytes = reader.ReadBytes(SectorConstants.Size);

        var sectorMode = GetSectorMode(bytes);

        Assert.AreEqual(sectorModeExpected, sectorMode);

        TestContext.WriteLine(sectorMode);
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static SectorMode GetSectorMode(in Span<byte> bytes)
    {
        ReadOnlySpan<byte> sync = stackalloc byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };

        if (!bytes[..12].SequenceEqual(sync))
        {
            return SectorMode.Audio; // TODO add an extra check against address
        }

        var mode = bytes[15];

        switch (mode)
        {
            case 0:
                return SectorMode.Mode0;
            case 1:
                return SectorMode.Mode1;
            case 2:

                var form1EdcSlice = bytes.Slice(SectorMode2Form1.EdcPosition, sizeof(uint));

                var form1Edc = BinaryPrimitives.ReadUInt32LittleEndian(form1EdcSlice);

                const int form1DataPosition = SectorMode2Form1.SubHeaderPosition;

                const int form1DataLength = SectorMode2Form1.SubHeaderSize + SectorMode2Form1.UserDataSize;

                var form1Data = bytes.Slice(form1DataPosition, form1DataLength);

                var form1EdcResult = EdcUtility.Compute(form1Data);

                if (form1EdcResult == form1Edc)
                {
                    return SectorMode.Mode2Form1;
                }

                var form2EdcSlice = bytes.Slice(SectorMode2Form2.ReservedOrEdcPosition, sizeof(uint));

                var form2Edc = BinaryPrimitives.ReadUInt32LittleEndian(form2EdcSlice);

                const int form2DataPosition = SectorMode2Form2.SubHeaderPosition;

                const int form2DataLength = SectorMode2Form2.SubHeaderSize + SectorMode2Form2.UserDataSize;

                var form2Data = bytes.Slice(form2DataPosition, form2DataLength);

                var form2EdcResult = EdcUtility.Compute(form2Data);

                if (form2EdcResult == form2Edc)
                {
                    return SectorMode.Mode2Form2;
                }

                const SectorMode formLess = SectorMode.Mode2FormLess;

                return formLess;
            default:
                throw new NotSupportedException($"Unknown sector mode: {mode}");
        }
    }

    private static bool CheckEdc<T>(Span<byte> bytes) where T : struct, ISector
    {
        var sectorMode1 = MemoryMarshal.Read<T>(bytes);

        var edc = sectorMode1.GetEdc();

        if (edc == 0)
        {
            return false; // TODO this is hack for mode 0, should be better
        }

        var computeEdc = sectorMode1.GetEdcSum();

        return computeEdc == edc;
    }
}