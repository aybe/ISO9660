using System.Numerics;

namespace ISO9660.Tests.FileSystem;

public interface IIsoNumber1<out T> where T : INumber<T>
{
    T Value { get; }
}