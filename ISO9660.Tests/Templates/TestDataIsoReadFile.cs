using ISO9660.WorkInProgress;

namespace ISO9660.Tests.Templates;

public record TestDataIsoReadFile(string Source, string Target, string Sha256, DiscReadFileMode Mode) : TestData;