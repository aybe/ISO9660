using System.Diagnostics.CodeAnalysis;

namespace CDROMTools.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public partial class NativeConstants
    {
        public const int MAXIMUM_NUMBER_TRACKS = 100;

        public const int FILE_ATTRIBUTE_NORMAL = 128;

        public const int ERROR_IO_PENDING = 997;

        public const int FILE_FLAG_OVERLAPPED = 1073741824;

        public const uint GENERIC_READ = 0x80000000;

        public const int FILE_SHARE_WRITE = 2;

        public const int FILE_SHARE_READ = 1;

        public const int OPEN_EXISTING = 3;

        public const int CD_BLOCKS_PER_SECOND = 75;

        public const int CD_RAW_READ_C2_SIZE = 296;

        public const int CD_RAW_READ_SUBCODE_SIZE = 96;

        public const int CD_RAW_SECTOR_SIZE = 2352;

        public const int CD_RAW_SECTOR_WITH_C2_SIZE = CD_RAW_SECTOR_SIZE + CD_RAW_READ_C2_SIZE;

        public const int CD_RAW_SECTOR_WITH_SUBCODE_SIZE = CD_RAW_SECTOR_SIZE + CD_RAW_READ_SUBCODE_SIZE;

        public const int CD_RAW_SECTOR_WITH_C2_AND_SUBCODE_SIZE =
            CD_RAW_SECTOR_SIZE + CD_RAW_READ_C2_SIZE + CD_RAW_READ_SUBCODE_SIZE;

        public const int METHOD_BUFFERED = 0;

        public const int METHOD_IN_DIRECT = 1;

        public const int METHOD_OUT_DIRECT = 2;

        public const int METHOD_NEITHER = 3;

        public const int METHOD_DIRECT_TO_HARDWARE = METHOD_IN_DIRECT;

        public const int METHOD_DIRECT_FROM_HARDWARE = METHOD_OUT_DIRECT;

        public const int FILE_ANY_ACCESS = 0;

        public const int FILE_SPECIAL_ACCESS = FILE_ANY_ACCESS;

        public const int FILE_READ_ACCESS = 1;

        public const int FILE_WRITE_ACCESS = 2;

        public const int IOCTL_CDROM_BASE = FILE_DEVICE_CD_ROM;

        public const int FILE_DEVICE_CD_ROM = 2;

        public static uint IOCTL_CDROM_UNLOAD_DRIVER
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0402, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_READ_TOC
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0000, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_SEEK_AUDIO_MSF
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0001, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_STOP_AUDIO
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0002, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_PAUSE_AUDIO
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0003, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_RESUME_AUDIO
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0004, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_GET_VOLUME
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0005, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_PLAY_AUDIO_MSF
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0006, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_SET_VOLUME
            => CTL_CODE(IOCTL_CDROM_BASE, 0x000A, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_READ_Q_CHANNEL
            => CTL_CODE(IOCTL_CDROM_BASE, 0x000B, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_GET_LAST_SESSION
            => CTL_CODE(IOCTL_CDROM_BASE, 0x000E, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_RAW_READ
            => CTL_CODE(IOCTL_CDROM_BASE, 0x000F, METHOD_OUT_DIRECT, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_DISK_TYPE
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0010, METHOD_BUFFERED, FILE_ANY_ACCESS);

        public static uint IOCTL_CDROM_GET_DRIVE_GEOMETRY
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0013, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_GET_DRIVE_GEOMETRY_EX
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0014, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_READ_TOC_EX
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0015, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_GET_CONFIGURATION
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0016, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_EXCLUSIVE_ACCESS
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0017, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);

        public static uint IOCTL_CDROM_SET_SPEED
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0018, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_GET_INQUIRY_DATA
            => CTL_CODE(IOCTL_CDROM_BASE, 0x0019, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_ENABLE_STREAMING
            => CTL_CODE(IOCTL_CDROM_BASE, 0x001A, METHOD_BUFFERED, FILE_READ_ACCESS);

        public static uint IOCTL_CDROM_SEND_OPC_INFORMATION
            => CTL_CODE(IOCTL_CDROM_BASE, 0x001B, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);

        public static uint IOCTL_CDROM_GET_PERFORMANCE
            => CTL_CODE(IOCTL_CDROM_BASE, 0x001C, METHOD_BUFFERED, FILE_READ_ACCESS);
   public static uint IOCTL_SCSI_PASS_THROUGH_DIRECT
            => CTL_CODE(IOCTL_SCSI_BASE, 0x0405, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);

        private static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return deviceType << 16 | access << 14 | function << 2 | method;
        }

        public const int SCSI_IOCTL_DATA_IN = 1;    /// IOCTL_SCSI_BASE -> FILE_DEVICE_CONTROLLER
        public const int IOCTL_SCSI_BASE = NativeConstants.FILE_DEVICE_CONTROLLER;

        /// FILE_DEVICE_CONTROLLER -> 0x00000004
        public const int FILE_DEVICE_CONTROLLER = 4;
    }
}