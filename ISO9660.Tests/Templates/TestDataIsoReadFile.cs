#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using ISO9660.WorkInProgress;

namespace ISO9660.Tests.Templates;

public sealed class TestDataIsoReadFile : TestData
{
    public string Source { get; set; }

    public string Target { get; set; }

    public string Sha256 { get; set; }

    public bool Cooked { get; set; }
}