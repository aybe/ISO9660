using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using ISO9660.Logical;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.CLI;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var source = new Argument<string>(
            "source",
            "ISO-9660 image to read from, either .cue or .iso file.");

        var target = new Argument<string>(
            "target",
            "File to read in source image, e.g. /SYSTEM.CNF.");

        var output = new Argument<string>(
            "output",
            "Directory where to write the target file to.");

        var cooked = new Argument<bool>(
            "cooked",
            "Extract file as user data (true), or in raw mode (false).");

        var list = new Command("list", "File list mode.")
        {
            source
        };

        list.SetHandler(List, source);

        var read = new Command("read", "File read mode.")
        {
            source, target, output, cooked
        };

        read.SetHandler(Read, source, target, output, cooked);

        var root = new RootCommand("ISO-9660 file system reader.")
        {
            list, read
        };

        var code = await root.InvokeAsync(args).ConfigureAwait(false);

        return code;
    }

    private static ErrorCode Error(ErrorCode error)
    {
        var message = error switch
        {
            ErrorCode.Success       => "The operation completed successfully.",
            ErrorCode.InvalidSource => "Source image file could not be found.",
            ErrorCode.InvalidFormat => "Source image file format is not valid.",
            ErrorCode.InvalidSystem => "Source image file system could not be read.",
            ErrorCode.InvalidTarget => "Target file could not be found in source image.",
            ErrorCode.InvalidOutput => "Output directory is not a valid path.",
            ErrorCode.Failed        => "Failed to read file from image.",
            _                       => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };

        if (error is not ErrorCode.Success)
        {
            Console.WriteLine(message);
        }

        return error;
    }

    private static async Task<ErrorCode> List(string source)
    {
        using var workspace = Workspace.TryOpen(source);

        if (workspace.Error is not ErrorCode.Success)
        {
            return Error(workspace.Error);
        }

        await Task.Run(() =>
        {
            var stack = new Stack<IsoFileSystemEntryDirectory>();

            stack.Push(workspace.System!.RootDirectory);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                foreach (var file in pop.Files)
                {
                    Console.WriteLine(file.FullName);
                }

                foreach (var item in pop.Directories.AsEnumerable().Reverse())
                {
                    stack.Push(item);
                }
            }
        });

        return Error(ErrorCode.Success);
    }

    private static async Task<ErrorCode> Read(string source, string target, string output, bool cooked)
    {
        using var workspace = Workspace.TryOpen(source);

        if (workspace.Error is not ErrorCode.Success)
        {
            return Error(workspace.Error);
        }

        if (workspace.System!.TryFindFile(target, out var file) == false)
        {
            return Error(ErrorCode.InvalidTarget);
        }

        try
        {
            Directory.CreateDirectory(output);
        }
        catch (Exception)
        {
            return Error(ErrorCode.InvalidOutput);
        }

        var path = Path.GetFullPath(Path.Combine(output, file.FileName));

        await using var stream = File.Create(path);

        try
        {
            var disc = workspace.Disc!;

            if (cooked)
            {
                await disc.ReadFileUserAsync(file, stream);
            }
            else
            {
                await disc.ReadFileRawAsync(file, stream);
            }
        }
        catch (Exception)
        {
            return Error(ErrorCode.Failed);
        }

        return Error(ErrorCode.Success);
    }
}

internal sealed class Workspace : Disposable
{
    private Workspace()
    {
    }

    public Disc? Disc { get; private set; }

    public IsoFileSystem? System { get; private set; }

    public ErrorCode Error { get; private set; }

    protected override void DisposeManaged()
    {
        System?.Dispose();

        Disc?.Dispose();
    }

    public static Workspace TryOpen(string path)
    {
        var workspace = new Workspace();

        if (!File.Exists(path))
        {
            workspace.Error = ErrorCode.InvalidSource;

            return workspace;
        }

        if (!TryOpenDisc(path, out var disc))
        {
            workspace.Error = ErrorCode.InvalidFormat;

            return workspace;
        }

        workspace.Disc = disc;

        if (!TryReadDisc(disc, out var ifs))
        {
            workspace.Error = ErrorCode.InvalidSystem;

            return workspace;
        }

        workspace.System = ifs;

        workspace.Error = ErrorCode.Success;

        return workspace;
    }

    private static bool TryOpenDisc(string source, [NotNullWhen(true)] out Disc? result)
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

    private static bool TryReadDisc(Disc disc, [NotNullWhen(true)] out IsoFileSystem? result)
    {
        try
        {
            result = IsoFileSystem.Read(disc);
        }
        catch (Exception)
        {
            result = default;
        }

        return result != null;
    }
}

internal enum ErrorCode
{
    Success = 0,
    InvalidSource = 2,
    InvalidFormat = 3,
    InvalidSystem = 4,
    InvalidTarget = 5,
    InvalidOutput = 6,
    Failed = 7
}