using System.Numerics;

namespace WipeoutInstaller.Iso9660;

public interface IIsoNumber1<out T> where T : INumber<T>
{
    T Value { get; }
}