using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental.RockRidge;

public sealed class PosixDeviceNumber : SystemUseEntry
{
    public PosixDeviceNumber(BinaryReader reader)
        : base(reader)
    {
        DeviceNumberHigh = new Iso733(reader);

        DeviceNumberLow = new Iso733(reader);
    }

    public Iso733 DeviceNumberHigh { get; }

    public Iso733 DeviceNumberLow { get; }
}