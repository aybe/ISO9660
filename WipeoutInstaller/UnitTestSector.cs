using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.WorkInProgress;

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
        Assert.AreEqual(SectorConstants.Size, Unsafe.SizeOf<SectorMode2>());
    }

    [TestMethod]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).bin", 16, SectorModeExtended.Mode1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 16, SectorModeExtended.Mode2Form1)]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.bin", 1605, SectorModeExtended.Mode2Form2)]
    public void TestSectorModeUsingEdc(string path, int sectorIndex, SectorModeExtended sectorMode)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        Assert.AreEqual(0, stream.Length % SectorConstants.Size);

        stream.Position = sectorIndex * SectorConstants.Size;

        var bytes = reader.ReadBytes(SectorConstants.Size);

        var edc = sectorMode switch
        {
            SectorModeExtended.Audio         => throw new NotImplementedException(),
            SectorModeExtended.Mode0         => throw new NotImplementedException(),
            SectorModeExtended.Mode1         => CheckEdc<SectorMode1>(bytes),
            SectorModeExtended.Mode2Form1    => CheckEdc<SectorMode2Form1>(bytes),
            SectorModeExtended.Mode2Form2    => CheckEdc<SectorMode2Form2>(bytes),
            SectorModeExtended.Mode2FormLess => throw new NotImplementedException(),
            _                        => throw new ArgumentOutOfRangeException(nameof(sectorMode), sectorMode, null)
        };

        Assert.IsTrue(edc);
    }

    [TestMethod]
    [DataRow(@"C:\Temp\UFO - Enemy Unknown (1994)(MicroProse).bin", 16, SectorModeExtended.Mode1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 16, SectorModeExtended.Mode2Form1)]
    [DataRow(@"C:\Temp\WipEout (Europe) (v1.1).bin", 27170, SectorModeExtended.Audio)]
    [DataRow(@"C:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.bin", 1605, SectorModeExtended.Mode2Form2)]
    public void TestSectorModeUsingInspection(string path, int sectorIndex, SectorModeExtended sectorModeExpected)
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
    private static SectorModeExtended GetSectorMode(in Span<byte> bytes)
    {
        ReadOnlySpan<byte> sync = stackalloc byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };

        if (!bytes[..12].SequenceEqual(sync))
        {
            return SectorModeExtended.Audio; // TODO add an extra check against address
        }

        var mode = bytes[15];

        switch (mode)
        {
            case 0:
                return SectorModeExtended.Mode0;
            case 1:
                return SectorModeExtended.Mode1;
            case 2:

                var form1EdcSlice = bytes.Slice(SectorMode2Form1.EdcPosition, sizeof(uint));

                var form1Edc = BinaryPrimitives.ReadUInt32LittleEndian(form1EdcSlice);

                const int form1DataPosition = SectorMode2Form1.SubHeaderPosition;

                const int form1DataLength = SectorMode2Form1.SubHeaderSize + SectorMode2Form1.UserDataSize;

                var form1Data = bytes.Slice(form1DataPosition, form1DataLength);

                var form1EdcResult = EdcUtility.Compute(form1Data);

                if (form1EdcResult == form1Edc)
                {
                    return SectorModeExtended.Mode2Form1;
                }

                var form2EdcSlice = bytes.Slice(SectorMode2Form2.ReservedOrEdcPosition, sizeof(uint));

                var form2Edc = BinaryPrimitives.ReadUInt32LittleEndian(form2EdcSlice);

                const int form2DataPosition = SectorMode2Form2.SubHeaderPosition;

                const int form2DataLength = SectorMode2Form2.SubHeaderSize + SectorMode2Form2.UserDataSize;

                var form2Data = bytes.Slice(form2DataPosition, form2DataLength);

                var form2EdcResult = EdcUtility.Compute(form2Data);

                if (form2EdcResult == form2Edc)
                {
                    return SectorModeExtended.Mode2Form2;
                }

                const SectorModeExtended formLess = SectorModeExtended.Mode2FormLess;

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