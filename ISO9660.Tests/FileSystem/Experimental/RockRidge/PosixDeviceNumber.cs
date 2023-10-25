using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class PosixDeviceNumber : SystemUseEntry
{
    public PosixDeviceNumber(BinaryReader reader)
        : base(reader)
    {
        DeviceNumberHigh = reader.ReadIso733();

        DeviceNumberLow = reader.ReadIso733();
    }

    public uint DeviceNumberHigh { get; }

    public uint DeviceNumberLow { get; }
}