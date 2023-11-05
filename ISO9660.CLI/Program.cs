using System.CommandLine;
using ISO9660.Logical;
using ISO9660.Physical;

namespace ISO9660.CLI;

internal static class Program
{
    public static int Main(string[] args)
    {
        var source = new Option<string>(
            "--source",
            "Source image, either .cue or .iso file.")
        {
            IsRequired = true
        };

        var target = new Option<string>(
            "--target",
            "Target file to read, e.g. /SYSTEM.CNF.")
        {
            IsRequired = true
        };

        var cooked = new Option<bool>(
            "--cooked",
            "Extract as user data or in raw mode.")
        {
            IsRequired = true
        };

        var output = new Option<string>(
            "--output",
            "Output directory to write file to.")
        {
            IsRequired = true
        };

        var command = new RootCommand(
            "Extracts a file from an ISO-9660 file system.");

        command.AddOption(source);
        command.AddOption(target);
        command.AddOption(cooked);
        command.AddOption(output);

        command.SetHandler(Handle, source, target, cooked, output);

        var task = command.InvokeAsync(args);

        var code = task.Result;

        return code;
    }

    private static async Task Handle(string source, string target, bool cooked, string output)
    {
        if (File.Exists(source) == false)
        {
            throw new InvalidOperationException("Source file does not exist.");
        }

        var type = Path.GetExtension(source).ToLowerInvariant();

        using var disc = type switch
        {
            ".cue" => Disc.FromCue(source),
            ".iso" => Disc.FromIso(source),
            _      => throw new InvalidOperationException("Source file type not supported.")
        };

        var ifs = IsoFileSystem.Read(disc);

        if (ifs.TryFindFile(target, out var file) == false)
        {
            throw new InvalidOperationException("Target file not found in source image.");
        }

        Directory.CreateDirectory(output);

        var path = Path.Combine(output, file.FileName);

        await using var stream = File.Create(path);

        if (cooked)
        {
            disc.ReadFileUser(file, stream);
        }
        else
        {
            disc.ReadFileRaw(file, stream);
        }
    }
}