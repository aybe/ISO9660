namespace WipeoutInstaller.Templates;

public record TestDataIsoReadFile(string Source, string Target, string Sha256, DiscReadFileMode Mode) : TestData;