using System;

namespace CDROMTools.Interop
{
    [Flags]
    public enum SubChannelQControlBits : byte
    {
        /// <summary>
        ///     Flag indicates a REGULAR audio track with 2 channels.
        /// </summary>
        None = 0x0,

        AudioWithPreemphasis = 0x1,

        DigitalCopyPermitted = 0x2,

        /// <summary>
        ///     Flag indicates a DATA track.
        /// </summary>
        AudioDataTrack = 0x4,

        TwoFourChannelAudio = 0x8
    }
}