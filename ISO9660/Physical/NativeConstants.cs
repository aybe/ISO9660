namespace ISO9660.Physical;

internal static class NativeConstants
{
    #region devioctl.h

    public const uint FILE_DEVICE_CD_ROM = 0x00000002;

    public const int FILE_DEVICE_CONTROLLER = 0x00000004;

    public const int FILE_DEVICE_MASS_STORAGE = 0x0000002d;

    public const int FILE_ANY_ACCESS = 0;

    public const uint FILE_READ_ACCESS = 0x0001;

    public const int FILE_WRITE_ACCESS = 0x0002;

    public const uint METHOD_BUFFERED = 0;

    public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
    {
        return DeviceType << 16 | Access << 14 | Function << 2 | Method;
    }

    #endregion

    #region ntddcdrm.h

    public const int CDROM_READ_TOC_EX_FORMAT_TOC = 0x00;

    public const uint IOCTL_CDROM_BASE = FILE_DEVICE_CD_ROM;

    public const int MAXIMUM_NUMBER_TRACKS = 100;

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddcdrm/ni-ntddcdrm-ioctl_cdrom_read_toc_ex
    /// </summary>
    public static uint IOCTL_CDROM_READ_TOC_EX { get; } = CTL_CODE(IOCTL_CDROM_BASE, 0x0015, METHOD_BUFFERED, FILE_READ_ACCESS);

    #endregion

    #region ntddscsi.h

    public const int IOCTL_SCSI_BASE = FILE_DEVICE_CONTROLLER;

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddscsi/ni-ntddscsi-ioctl_scsi_pass_through_direct
    /// </summary>
    public static uint IOCTL_SCSI_PASS_THROUGH_DIRECT
        => CTL_CODE(IOCTL_SCSI_BASE, 0x0405, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);

    public const int SCSI_IOCTL_DATA_IN = 1;

    #endregion

    #region ntddstor.h

    public const int IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;

    /// <summary>
    ///     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntddstor/ni-ntddstor-ioctl_storage_query_property
    /// </summary>
    public static uint IOCTL_STORAGE_QUERY_PROPERTY
        => CTL_CODE(IOCTL_STORAGE_BASE, 0x0500, METHOD_BUFFERED, FILE_ANY_ACCESS);

    #endregion

    #region winerror.h

    public const int ERROR_SUCCESS = 0;

    public const int ERROR_IO_PENDING = 997;

    #endregion
}