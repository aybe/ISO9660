using System.Numerics;

namespace WipeoutInstaller.ISO9660;

public interface IIsoNumber2<out T> where T : INumber<T>
{
    T Value1 { get; }

    T Value2 { get; }
}