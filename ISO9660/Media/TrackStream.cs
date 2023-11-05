using Whatever.Extensions;

namespace ISO9660.Media;

internal sealed class TrackStream : Stream
    // better than exposing track stream which might represent N tracks
{
    private readonly Track Track;

    private readonly int UserDataLength;

    private int SectorNumber;

    private int SectorOffset;

    public TrackStream(in Track track, in int index)
    {
        Track = track;

        UserDataLength = Track.Sector.GetUserDataLength();

        Length = (Track.Length - Track.Position) * UserDataLength;

        Position = index * UserDataLength;
    }

    public override bool CanRead { get; } = true;

    public override bool CanSeek { get; } = false;

    public override bool CanWrite { get; } = false;

    public override long Length { get; }

    public override long Position
    {
        get => SectorNumber * UserDataLength + SectorOffset;
        set
        {
            SectorNumber = (value / UserDataLength).ToInt32();
            SectorOffset = (value % UserDataLength).ToInt32();
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

    public override string ToString()
    {
        return $"{nameof(SectorNumber)}: {SectorNumber}, {nameof(SectorOffset)}: {SectorOffset}";
    }
}