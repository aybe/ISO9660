using ISO9660.Tests.Extensions;

namespace ISO9660.Tests.WorkInProgress;

internal sealed class DiscTrackStream : Stream
    // better than exposing track stream which might represent N tracks
{
    private readonly DiscTrack Track;

    private readonly int UserDataLength;

    private int SectorNumber;

    private int SectorOffset;

    public DiscTrackStream(in DiscTrack track, in int index)
    {
        Track = track;

        UserDataLength = Track.Sector.GetUserDataLength();

        Position = index * UserDataLength;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => (Track.Length - Track.Position) * UserDataLength;

    public override long Position
    {
        get => SectorNumber * UserDataLength + SectorOffset;
        set
        {
            var position = value.ToInt32();

            SectorNumber = position / UserDataLength;
            SectorOffset = position % UserDataLength;
        }
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] bytes, int index, int count)
    {
        ValidateBufferArguments(bytes, index, count);

        var total = 0;

        var input = Span<byte>.Empty;

        while (count > 0 && SectorNumber < Track.Length)
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
}