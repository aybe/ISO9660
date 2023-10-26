﻿using JetBrains.Annotations;

namespace ISO9660.Tests.WorkInProgress;

public unsafe struct SectorCooked2336 : ISector
{
    private const int UserDataLength = 2336;

    private const int UserDataPosition = 0;

    [UsedImplicitly]
    public fixed byte UserData[UserDataLength];

    public readonly int Size => 2336;

    public Span<byte> AsByteSpan()
    {
        return ISector.AsByteSpan(ref this);
    }

    public Span<byte> GetUserData()
    {
        return ISector.GetSlice(ref this, UserDataPosition, UserDataLength);
    }

    public readonly int GetUserDataLength()
    {
        return UserDataLength;
    }
}