namespace WipeoutInstaller.ISO9660;

[Flags]
public enum IsoStringFlags
{
    ACharacters = 1 << 0,
    DCharacters = 1 << 1,
    Separator1 = 1 << 2,
    Separator2 = 1 << 3,
    Byte00 = 1 << 4,
    Byte01 = 1 << 5
}