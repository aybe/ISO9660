using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
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

    private static async Task<Result> Handle(string source, string target, bool cooked, string output)
    {
        if (File.Exists(source) == false)
        {
            Console.WriteLine("Source image file could not be found.");
            return Result.InvalidSource;
        }

        if (TryOpenDisc(source, out var result) == false)
        {
            Console.WriteLine("Source image file is not supported.");
            return Result.InvalidSourceFormat;
        }

        using var disc = result;

        IsoFileSystem ifs;

        try
        {
            ifs = IsoFileSystem.Read(disc);
        }
        catch (Exception e)
        {
            Console.WriteLine("File system could not be read from source image.");
            Console.WriteLine(e);
            return Result.InvalidSystem;
        }

        if (ifs.TryFindFile(target, out var file) == false)
        {
            Console.WriteLine("Target file could not be found in source image.");
            return Result.InvalidTarget;
        }

        try
        {
            Directory.CreateDirectory(output);
        }
        catch (Exception e)
        {
            Console.WriteLine("Output directory is not a valid path.");
            Console.WriteLine(e);
            return Result.InvalidOutput;
        }

        var path = Path.GetFullPath(Path.Combine(output, file.FileName));

        await using var stream = File.Create(path);

        try
        {
            if (cooked)
            {
                disc.ReadFileUserAsync(file, stream);
            }
            else
            {
                disc.ReadFileRawAsync(file, stream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Target file could not be read from source image.");
            Console.WriteLine(e);
            return Result.ReadingFailed;
        }

        return Result.Success;
    }

    private static bool TryOpenDisc(string source, [MaybeNullWhen(false)] out Disc result)
    {
        var type = Path.GetExtension(source).ToLowerInvariant();

        result = type switch
        {
            ".cue" => Disc.FromCue(source),
            ".iso" => Disc.FromIso(source),
            _      => null
        };

        return result != null;
    }

    private enum Result
    {
        Success = 0,
        InvalidSource = 1,
        InvalidSourceFormat = 2,
        InvalidSystem = 3,
        InvalidTarget = 4,
        InvalidOutput = 5,
        ReadingFailed = 6
    }
}