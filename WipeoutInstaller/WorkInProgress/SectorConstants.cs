namespace WipeoutInstaller.WorkInProgress;

public static class SectorConstants
    // CD-ROM
    // https://en.wikipedia.org/wiki/CD-ROM
    // ECMA 130
    // https://www.ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf 
    // SYSTEM DESCRIPTION CD-ROM XA, PHILIPS/SONY, May 1991
    // https://archive.org/details/xa-10-may-1991
    // CD Cracking Uncovered Protection Against Unsanctioned CD Copying
    // https://archive.org/details/CDCrackingUncoveredProtectionAgainstUnsanctionedCDCopyingKrisKaspersky
{
    public const int Size = 2352;

    public const int SyncPosition = 0;
    public const int SyncSize = 12;

    public const int HeaderPosition = SyncPosition + SyncSize;
    public const int HeaderSize = 4;

    public const int SubHeaderPosition = HeaderPosition + HeaderSize;
    public const int SubHeaderSize = 8;

    public const int UserDataPosition = SubHeaderPosition + SubHeaderSize;
    public const int UserDataSize = 2328;
}