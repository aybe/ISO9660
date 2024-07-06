using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace ISO9660.Physical;

internal static partial class NativeTypes
{
    #region ntddcdrm.h

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddcdrm/ns-ntddcdrm-_cdrom_read_toc_ex
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CDROM_READ_TOC_EX
    {
        public byte BitVector1;

        public byte SessionTrack;

        public byte Reserved2;

        public byte Reserved3;

        public byte Format
        {
            get => (byte)(BitVector1 >> 0 & 0b1111);
            set => BitVector1 |= (byte)((value & 0b1111) << 0);
        }

        public byte Reserved1
        {
            get => (byte)(BitVector1 >> 4 & 0b111);
            set => BitVector1 |= (byte)((value & 0b111) << 4);
        }

        public byte Msf
        {
            get => (byte)(BitVector1 >> 7 & 0b1);
            set => BitVector1 |= (byte)((value & 0b1) << 7);
        }

        public override string ToString()
        {
            return $"{nameof(SessionTrack)}: {SessionTrack}, {nameof(Format)}: {Format}, {nameof(Msf)}: {Msf}";
        }
    }

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddcdrm/ns-ntddcdrm-_cdrom_toc
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CDROM_TOC
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Length;

        public byte FirstTrack;

        public byte LastTrack;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.MAXIMUM_NUMBER_TRACKS)]
        public TRACK_DATA[] TrackData;

        public override string ToString()
        {
            return $"{nameof(Length)}: {BinaryPrimitives.ReadUInt16BigEndian(Length)}, {nameof(FirstTrack)}: {FirstTrack}, {nameof(LastTrack)}: {LastTrack}";
        }
    }

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddcdrm/ns-ntddcdrm-_track_data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TRACK_DATA
    {
        public byte Reserved;

        public byte BitVector1;

        public byte TrackNumber;

        public byte Reserved1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Address;

        public byte Control => (byte)(BitVector1 & 0b00001111);

        public byte Adr => (byte)(BitVector1 >> 4 & 0b1111);

        public override string ToString()
        {
            return $"{nameof(TrackNumber)}: {TrackNumber}, {nameof(Control)}: {Control}, {nameof(Adr)}: {Adr}, {nameof(Address)}: {BinaryPrimitives.ReadInt32BigEndian(Address)}";
        }
    }

    #endregion
}

internal static partial class NativeTypes
{
    #region ntddscsi.h

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddscsi/ns-ntddscsi-_scsi_pass_through_direct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SCSI_PASS_THROUGH_DIRECT
    {
        public SCSI_PASS_THROUGH_DIRECT(byte cdbLength)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cdbLength, 16);

            Length = (ushort)Marshal.SizeOf<SCSI_PASS_THROUGH_DIRECT>();
            ScsiStatus = 0;
            PathId = 0;
            TargetId = 0;
            Lun = 0;
            CdbLength = cdbLength;
            SenseInfoLength = 0;
            DataIn = 0;
            DataTransferLength = 0;
            TimeOutValue = 0;
            DataBuffer = nint.Zero;
            SenseInfoOffset = 0;
            Cdb = new byte[16];
        }

        public ushort Length;

        public byte ScsiStatus;

        public byte PathId;

        public byte TargetId;

        public byte Lun;

        public byte CdbLength;

        public byte SenseInfoLength;

        public byte DataIn;

        public uint DataTransferLength;

        public uint TimeOutValue;

        public nint DataBuffer;

        public uint SenseInfoOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Cdb;
    }

    #endregion
}

internal static partial class NativeTypes
{
    #region ntddstor.h

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddstor/ns-ntddstor-_storage_property_query
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_PROPERTY_QUERY
    {
        public STORAGE_PROPERTY_ID PropertyId;

        public STORAGE_QUERY_TYPE QueryType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] AdditionalParameters;
    }

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddstor/ns-ntddstor-_storage_adapter_descriptor
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_ADAPTER_DESCRIPTOR
    {
        public uint Version;

        public uint Size;

        public uint MaximumTransferLength;

        public uint MaximumPhysicalPages;

        public uint AlignmentMask;

        public byte AdapterUsesPio;

        public byte AdapterScansDown;

        public byte CommandQueueing;

        public byte AcceleratedTransfer;

        public byte BusType;

        public ushort BusMajorVersion;

        public ushort BusMinorVersion;

        public byte SrbType;

        public byte AddressType;
    }

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddstor/ne-ntddstor-storage_property_id
    /// </summary>
    public enum STORAGE_PROPERTY_ID
    {
        StorageDeviceProperty = 0,
        StorageAdapterProperty,
        StorageDeviceIdProperty,
        StorageDeviceUniqueIdProperty,
        StorageDeviceWriteCacheProperty,
        StorageMiniportProperty,
        StorageAccessAlignmentProperty,
        StorageDeviceSeekPenaltyProperty,
        StorageDeviceTrimProperty,
        StorageDeviceWriteAggregationProperty,
        StorageDeviceDeviceTelemetryProperty,
        StorageDeviceLBProvisioningProperty,
        StorageDevicePowerProperty,
        StorageDeviceCopyOffloadProperty,
        StorageDeviceResiliencyProperty,
        StorageDeviceMediumProductType,
        StorageAdapterRpmbProperty,
        StorageAdapterCryptoProperty,
        StorageDeviceTieringProperty,
        StorageDeviceFaultDomainProperty,
        StorageDeviceClusportProperty,
        StorageDeviceDependantDevicesProperty,
        StorageDeviceIoCapabilityProperty = 48,
        StorageAdapterProtocolSpecificProperty,
        StorageDeviceProtocolSpecificProperty,
        StorageAdapterTemperatureProperty,
        StorageDeviceTemperatureProperty,
        StorageAdapterPhysicalTopologyProperty,
        StorageDevicePhysicalTopologyProperty,
        StorageDeviceAttributesProperty,
        StorageDeviceManagementStatus,
        StorageAdapterSerialNumberProperty,
        StorageDeviceLocationProperty,
        StorageDeviceNumaProperty,
        StorageDeviceZonedDeviceProperty,
        StorageDeviceUnsafeShutdownCount,
        StorageDeviceEnduranceProperty,
        StorageDeviceLedStateProperty,
        StorageDeviceSelfEncryptionProperty = 64,
        StorageFruIdProperty,
        StorageStackProperty,
        StorageAdapterProtocolSpecificPropertyEx,
        StorageDeviceProtocolSpecificPropertyEx,
    }

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddstor/ne-ntddstor-_storage_query_type
    /// </summary>
    public enum STORAGE_QUERY_TYPE
    {
        PropertyStandardQuery = 0,
        PropertyExistsQuery,
        PropertyMaskQuery,
        PropertyQueryMaxDefined,
    }

    #endregion
}