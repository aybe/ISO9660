﻿namespace WipeoutInstaller.WorkInProgress;

internal sealed class DiscTrackStream : Stream
{
    private readonly Queue<byte> Queue;

    private readonly DiscTrack Track;

    private int Index;

    public DiscTrackStream(in DiscTrack track, in int index)
    {
        Queue = new Queue<byte>(4096);
        Track = track;
        Index = index;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length // TODO simplify
    {
        get
        {
            var sectors = Track.Length - Track.Position;

            var sectorSize = Track.Sector.Size;

            var bytes = sectors * sectorSize;

            return bytes;
        }
    }

    public override long Position // TODO simplify
    {
        get
        {
            var sector = Track.Sector;

            var sectorSize = sector.Size;

            var userDataPosition = sector.GetUserDataPosition();

            var userDataLength = sector.GetUserDataLength();

            var empty = Queue.Count == 0;

            var sectors = (empty ? Index : Index - 1) - Track.Position;

            var sectorStartByte = sectorSize * sectors + userDataPosition;

            var bytes = sectorStartByte + (empty ? 0 : userDataLength - Queue.Count);

            return bytes;
        }
        set => throw new NotImplementedException();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] bytes, int index, int count)
    {
        ValidateBufferArguments(bytes, index, count);

        var total = 0;

        while (count > 0)
        {
            if (Queue.Count == 0)
            {
                var sector = Track.ReadSector(Index);

                Index++;

                var data = sector.GetUserData();

                foreach (var item in data)
                {
                    Queue.Enqueue(item);
                }
            }

            var items = Math.Min(count, Queue.Count);

            while (items > 0)
            {
                bytes[index] = Queue.Dequeue();
                index++;
                count--;
                total++;
                items--;
            }
        }

        return total;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
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