// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnassignedReadonlyField

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WipeoutInstaller.Extensions;
using WipeoutInstaller.Iso9660;
using WipeoutInstaller.JSON;
using WipeoutInstaller.XA;

namespace WipeoutInstaller;

[TestClass]
public class UnitTestIso9660
{
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
            WriteLine($"Reading descriptor @ {stream.Position}");

            var sectorMode2 = ReadSectorMode2(reader);

            var descriptor = ReadVolumeDescriptor(sectorMode2);

            WriteLine(descriptor.GetType());

            descriptors.Add(descriptor);

            if (descriptor is VolumeDescriptorSetTerminator)
            {
                break;
            }
        }

        var settings = new JsonSerializerSettings
        {
            Formatting       = Formatting.Indented,
            ContractResolver = new BaseFirstContractResolver()
        };

        var json = JsonConvert.SerializeObject(descriptors, settings);

        WriteLine(json);

        throw new NotImplementedException(reader.BaseStream.Position.ToString());
    }

    private SectorMode2 ReadSectorMode2(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(SectorMode2.Size);

        var sector = MemoryMarshal.AsRef<SectorMode2>(bytes);

        Assert.IsTrue(sector.SubHeaderBlock.IsValid);
        WriteLine("Reading sector...");
        WriteLine(sector.Header);
        WriteLine(sector.SubHeaderBlock.Header1);
        WriteLine(sector.SubHeaderBlock.Header2);
        return sector;
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