using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ISO9660.Common;
using WipeoutInstaller.WorkInProgress;

namespace WipeoutInstaller;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UnitTestSector : UnitTestBase
{
    [TestMethod]
    [DataRow(@"D:\Temp\UFO - Enemy Unknown (1994)(MicroProse).bin", 16, SectorType.Mode1)]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Single.bin", 16, SectorType.Mode2Form1)]
    [DataRow(@"D:\Temp\WipEout (Europe) (v1.1) - Single.bin", 27170, SectorType.Audio)]
    [DataRow(@"D:\Temp\CD-I Demo Disc - Fall 1996 - Spring 1997.bin", 1605, SectorType.Mode2Form2)]
    public void TestSectorMode(string path, int sectorIndex, SectorType sectorTypeExpected)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);

        Assert.AreEqual(0, stream.Length % ISector.RawSize);

        stream.Position = sectorIndex * ISector.RawSize;

        var span = reader.ReadBytes(ISector.RawSize).AsSpan();

        var header = MemoryMarshal.Read<SectorHeader>(span.Slice(ISector.HeaderPosition, ISector.HeaderSize));

        var address = header.Address;

        var msf = new MSF(FromBcd(address.Minute), FromBcd(address.Second), FromBcd(address.Frame)); // TODO move FromBcd

        var lba = msf.ToLBA();

        var match = lba == sectorIndex;

        WriteLine(() => header);
        WriteLine(() => address);
        WriteLine(() => msf);
        WriteLine(() => lba);
        WriteLine(() => match);

        SectorType sectorType;

        if (match)
        {
            var sync = span.Slice(ISector.SyncPosition, ISector.SyncSize);

            ReadOnlySpan<byte> syncPattern = stackalloc byte[12] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };

            if (sync.SequenceEqual(syncPattern))
            {
                var sectorMode = header.Mode;

                WriteLine(() => sectorMode);

                sectorType = sectorMode switch
                {
                    SectorMode.Mode0 => SectorType.Mode0,
                    SectorMode.Mode1 => SectorType.Mode1,
                    SectorMode.Mode2 => SectorType.Mode2FormLess,
                    _                => throw new InvalidDataException(sectorMode.ToString())
                };

                var isMode2FormLess = sectorType is SectorType.Mode2FormLess;

                WriteLine(() => isMode2FormLess);

                if (isMode2FormLess)
                {
                    var form1EdcSlice = span.Slice(ISector.EdcPositionMode2Form1, sizeof(uint));

                    var form1Edc = BinaryPrimitives.ReadUInt32LittleEndian(form1EdcSlice);

                    const int form1DataPosition = ISector.SubHeaderPositionMode2Form1;

                    const int form1DataLength = ISector.SubHeaderSizeMode2Form1 + ISector.UserDataSizeMode2Form1;

                    var form1Data = span.Slice(form1DataPosition, form1DataLength);

                    var form1EdcResult = EdcUtility.Compute(form1Data);

                    if (form1EdcResult == form1Edc)
                    {
                        sectorType = SectorType.Mode2Form1;
                    }
                    else
                    {
                        var form2EdcSlice = span.Slice(ISector.EdcPositionMode2Form2, sizeof(uint));

                        var form2Edc = BinaryPrimitives.ReadUInt32LittleEndian(form2EdcSlice);

                        const int form2DataPosition = ISector.SubHeaderPositionMode2Form2;

                        const int form2DataLength = ISector.SubHeaderSizeMode2Form2 + ISector.UserDataSizeMode2Form2;

                        var form2Data = span.Slice(form2DataPosition, form2DataLength);

                        var form2EdcResult = EdcUtility.Compute(form2Data);

                        if (form2EdcResult == form2Edc)
                        {
                            sectorType = SectorType.Mode2Form2;
                        }
                        else
                        {
                            throw new InvalidDataException(); // TODO
                        }
                    }
                }
            }
            else
            {
                sectorType = SectorType.Audio; // TODO
            }
        }
        else
        {
            sectorType = SectorType.Audio; // TODO
        }

        WriteLine(() => sectorType);

        Assert.AreEqual(sectorTypeExpected, sectorType);
        return;

        static byte FromBcd(byte value) // todo move to msf?
        {
            var a = (byte)((value >> 0) & 0xF);
            var b = (byte)((value >> 4) & 0xF);

            if (a > 9 || b > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            var c = (byte)(a + b * 10);

            return c;
        }
    }

    [TestMethod]
    public void TestSizeOf()
    {
        Assert.AreEqual(3, Unsafe.SizeOf<SectorAddress>());
        Assert.AreEqual(4, Unsafe.SizeOf<SectorSubHeader>());
        Assert.AreEqual(8, Unsafe.SizeOf<SectorSubHeaderBlock>());

        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorAudio>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode0>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode1>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2Form1>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2Form2>());
        Assert.AreEqual(ISector.RawSize, Unsafe.SizeOf<SectorMode2FormLess>());
    }
}