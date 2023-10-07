using System;

namespace CDROMTools
{
    [Flags]
    public enum SectorMode2Form1SubHeaderSubMode:byte
    {
        EndOfRecord = 1,
        VideoBlock = 2,
        AudioBlock=4,
        DataBlock=8,
        TriggerBlock=16,
        Form2=32,
        RealTimeBlock=64,
        EndOfFile=128
    }
}