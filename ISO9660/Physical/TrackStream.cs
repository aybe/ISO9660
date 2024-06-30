using System.Runtime.CompilerServices;
using Whatever.Extensions;

namespace ISO9660.Physical;

/// <summary>
///     Track stream over user data, i.e. cooked stream.
/// </summary>
internal sealed class TrackStream : Stream
// that's because a track may be N tracks, e.g. bin/cue.
{
    private readonly Track Track;

    private readonly int UserDataLength; // TODO rename to SectorLength

    private int SectorNumber;

    private int SectorOffset;

    public TrackStream(in Track track, in int index)
    {
        Track = track;

        UserDataLength = Track.Sector.GetUserDataLength();

        Length = Track.Length * UserDataLength;

        Position = index * UserDataLength;
    }

    public override bool CanRead { get; } = true;

    public override bool CanSeek { get; } = true;

    public override bool CanWrite { get; } = false;

    public override long Length { get; }

    public override long Position
    {
        get => SectorNumber * UserDataLength + SectorOffset;
        set
        {
            ValidatePosition(value);

            SectorNumber = (value / UserDataLength).ToInt32();
            SectorOffset = (value % UserDataLength).ToInt32();
        }
    }

    public override void Flush()
    {
        // NOP
    }

    public override int Read(byte[] bytes, int index, int count)
    {
        ValidateBufferArguments(bytes, index, count);

        var total = 0;

        var input = Span<byte>.Empty;

        while (count > 0 && SectorNumber < Track.Position + Track.Length)
        {
            if (input.IsEmpty)
            {
                input = Track.ReadSector(SectorNumber).GetUserData();
            }

            var len = Math.Min(count, input.Length - SectorOffset);

            var src = input.Slice(SectorOffset, len);

            var dst = bytes.AsSpan(index, len);

            src.CopyTo(dst);

            total += len;
            count -= len;
            index += len;

            SectorOffset += len;

            if (SectorOffset < input.Length)
            {
                continue;
            }

            SectorNumber++;

            SectorOffset = 0;

            input = Span<byte>.Empty;
        }

        return total;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var position = origin switch
        {
            SeekOrigin.Begin   => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End     => Length - offset,
            _                  => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
        };

        ValidatePosition(position);

        Position = position;

        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        return $"{nameof(SectorNumber)}: {SectorNumber}, {nameof(SectorOffset)}: {SectorOffset}"; // TODO add UserDataLength
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidatePosition(long position, [CallerArgumentExpression(nameof(position))] string positionName = null!)
    {
        if (position < Track.Position || position >= Track.Position + Track.Length)
        {
            throw new ArgumentOutOfRangeException(positionName, position, null);
        }
    }
}