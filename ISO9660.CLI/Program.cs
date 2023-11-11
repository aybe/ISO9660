using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using ISO9660.Logical;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.CLI;

internal static class Program
{
    private static readonly TextProgressBar ProgressBar = new();

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

        var parser = new CommandLineBuilder(root)
            .UseDefaults()
            .UseExceptionHandler(OnException, 1)
            .Build();

        var result = await parser.InvokeAsync(args).ConfigureAwait(false);

        return result;
    }

    private static void OnException(Exception exception, InvocationContext context)
    {
        var inner = exception.InnerException;

        Console.WriteLine($"{exception.Message}{(inner == null ? string.Empty : $" ({inner.Message})")}");
    }

    private static async Task List(string source)
    {
        using var workspace = Workspace.TryOpen(source);

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
    }

    private static async Task Read(string source, string target, string output, bool cooked)
    {
        using var workspace = Workspace.TryOpen(source);

        if (!workspace.System!.TryFindFile(target, out var file))
        {
            throw new InvalidOperationException($"File could not be found in file system: '{target}'.");
        }

        try
        {
            Directory.CreateDirectory(output);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Directory could not be created: '{output}'.", e);
        }

        var path = Path.GetFullPath(Path.Combine(output, file.FileName));

        await using var stream = File.Create(path);

        try
        {
            var disc = workspace.Disc!;

            var progress = new SparseProgress<double>(OnProgressGetter, OnProgressSetter, OnProgressChanged);

            if (cooked)
            {
                await disc.ReadFileUserAsync(file, stream, progress);
            }
            else
            {
                await disc.ReadFileRawAsync(file, stream, progress);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"File could not be read from file system: '{target}'.", e);
        }
    }

    private static double OnProgressGetter(ref double s)
    {
        return s;
    }

    [SuppressMessage("ReSharper", "RedundantAssignment")]
    private static void OnProgressSetter(ref double s, double t)
    {
        s = t;
    }

    private static void OnProgressChanged(double value)
    {
        ProgressBar.Clear();
        ProgressBar.Update(value);
        Console.CursorLeft = 0;
        Console.Write(ProgressBar);
    }
}

internal sealed class Workspace : Disposable
{
    private Workspace()
    {
    }

    public Disc? Disc { get; private set; }

    public IsoFileSystem? System { get; private set; }

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
            throw new InvalidOperationException($"Image file could not be found: '{path}'.");
        }

        var extension = Path.GetExtension(path).ToLowerInvariant();

        try
        {
            workspace.Disc = extension switch
            {
                ".cue" => Disc.FromCue(path),
                ".iso" => Disc.FromIso(path),
                _      => throw new NotSupportedException($"Image file type is not supported: '{extension}'.")
            };
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Disc could not be read from image file.", e);
        }

        try
        {
            workspace.System = IsoFileSystem.Read(workspace.Disc);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("File system could not be read from disc.", e);
        }

        return workspace;
    }
}