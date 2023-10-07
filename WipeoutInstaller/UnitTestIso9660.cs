// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnassignedReadonlyField

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.Iso9660;
using WipeoutInstaller.XA;

namespace WipeoutInstaller;

[TestClass]
public class UnitTestIso9660
{
    private readonly bool PrintDirectoryRecord = false;

    private readonly bool PrintPathTableRecord = false;

    [PublicAPI]
    public required TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow(@"C:\Temp\Wipeout\WipEout (Europe) (v1.1)\WipEout (Europe) (v1.1) (Track 01).bin")]
    public void TestIsoReading(string path)
    {
        using var stream = File.OpenRead(path);

        using var reader = new BinaryReader(stream);

        stream.Position = 16 * SectorMode2.Size;

        var descriptors = new List<VolumeDescriptor>();

        while (true)
        {
            var sectorMode2 = ReadSectorMode2(reader);

            var descriptor = ReadVolumeDescriptor(sectorMode2);

            descriptors.Add(descriptor);

            if (descriptor is VolumeDescriptorSetTerminator)
            {
                break;
            }
        }

        var pvd = descriptors.OfType<PrimaryVolumeDescriptor>().Single();

        SetSector(pvd.LocationOfOccurrenceOfTypeLPathTable);

        using var pvdReader = ReadSector(pvd.LocationOfOccurrenceOfTypeLPathTable, reader).GetUserData()
            .ToBinaryReader();

        var records = new List<PathTableRecord>();
        var dictionary = new Dictionary<PathTableRecord, List<DirectoryRecord>>();
        while (pvdReader.BaseStream.Position < pvd.PathTableSize)
        {
            var record = new PathTableRecord(pvdReader);

            records.Add(record);
            dictionary.Add(record, new List<DirectoryRecord>());
            if (PrintPathTableRecord)
            {
                Print($"{record}, {record.ParentDirectoryNumber}, {record.LocationOfExtent}");
                PrintJson(record);
                Print();
            }
        }

        foreach (var record in records)
        {
            var recordExtent = record.LocationOfExtent;

            SetSector(recordExtent);
            if (PrintDirectoryRecord)
            {
                Print($"Reading records, sector {recordExtent}, user data @ {stream.Position}");

                Print($"\tIdentifiers: {record.LengthOfDirectoryIdentifier}");
            }

            for (;;)
            {
                var item = new DirectoryRecord(reader);

                if (item.LengthOfDirectoryRecord == 0)
                {
                    break;
                }

                dictionary[record].Add(item);
                if (PrintDirectoryRecord)
                {
                    Print(
                        $"\t\tName: {item.FileIdentifier}, Position: {item.LocationOfExtent}, Length: {item.DataLength}, Flags: {item.FileFlags}, Folder: {record.ParentDirectoryNumber}");

                    PrintJson(item);
                }
            }
        }
        
        IsoFileSystemEntry.Build(dictionary, records);

        throw new NotImplementedException(reader.BaseStream.Position.ToString("N0"));

        PrintJson(descriptors);

        void SetSector(int index)
        {
            stream.Position = index * SectorMode2.Size + SectorMode2.UserDataPosition;
        }
    }


    private SectorMode2 ReadSector(int number, BinaryReader reader)
    {
        reader.BaseStream.Position = number * SectorMode2.Size;
        return ReadSectorMode2(reader);
    }

    private SectorMode2 ReadSectorMode2(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(SectorMode2.Size);

        var sector = MemoryMarshal.AsRef<SectorMode2>(bytes);

        Assert.IsTrue(sector.SubHeaderBlock.IsValid);
        return sector;
        WriteLine("Reading sector...");
        WriteLine(sector.Header);
        WriteLine(sector.SubHeaderBlock.Header1);
        WriteLine(sector.SubHeaderBlock.Header2);
    }

    private void PrintJson(object? value)
    {
        var json = JsonUtility.ToJson(value);

        Print(json);
    }

    private void Print(object? value = null)
    {
        WriteLine(value);
    }

    private void WriteLine(object? value = null)
    {
        TestContext.WriteLine(value);
    }

    private static VolumeDescriptor ReadVolumeDescriptor(SectorMode2 sector)
    {
        var userData = sector.GetUserData();

        using var reader = new BinaryReader(new MemoryStream(userData));

        var descriptor = new VolumeDescriptor
        {
            VolumeDescriptorType    = reader.Read<VolumeDescriptorType>(), // 711
            StandardIdentifier      = reader.ReadStringAscii(5),
            VolumeDescriptorVersion = new Iso711(reader)
        };

        switch (descriptor.VolumeDescriptorType)
        {
            case VolumeDescriptorType.PrimaryVolumeDescriptor: // TODO expected
                return new PrimaryVolumeDescriptor(descriptor, reader);
            case VolumeDescriptorType.VolumeDescriptorSetTerminator: // TODO expected
                return new VolumeDescriptorSetTerminator(descriptor, reader);
            default:
                throw new NotImplementedException();
        }
    }
}