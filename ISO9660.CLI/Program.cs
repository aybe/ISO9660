using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using ISO9660.Logical;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.CLI;

internal static partial class Program
    // https://learn.microsoft.com/en-us/dotnet/standard/commandline/handle-termination
    // it isn't worth the effort to implement cancellation, too much work for so little
{
    private static readonly Argument<string> Source = new(
        "source",
        "Source image, .cue or .iso file.");

    public static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("CD-ROM image reader.")
        {
            BuildList(),
            BuildRead()
        };

        var parser = new CommandLineBuilder(root)
            .UseDefaults()
            .UseExceptionHandler((exception, _) => Console.WriteLine(exception), 1)
            .Build();

        var result = await parser.InvokeAsync(args).ConfigureAwait(false);

        return result;
    }

    private static void CreateDirectory(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Directory could not be created: '{path}'.", e);
        }
    }
}

internal static partial class Program
{
    private static SparseProgress<double> Progress { get; } =
        new(OnProgressGetter, OnProgressSetter, OnProgressChanged)
        {
            Digits = 2, Synchronous = true
        };

    private static TextProgressBar ProgressBar { get; } = new();

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

internal static partial class Program
{
    private static Command BuildList()
    {
        var command = new Command("list", "List mode.")
        {
            BuildListSystem(),
            BuildListTracks()
        };

        return command;
    }

    private static Command BuildListSystem()
    {
        var command = new Command("system", "Lists files in file system.")
        {
            Source
        };

        command.SetHandler(StartListSystem, Source);

        return command;
    }

    private static Command BuildListTracks()
    {
        var command = new Command("tracks", "Lists tracks in disc image.")
        {
            Source
        };

        command.SetHandler(StartListTracks, Source);

        return command;
    }

    private static async Task StartListSystem(string source)
    {
        using var workspace = Workspace.TryOpen(source);

        var sys = workspace.GetSystem();

        var stack = new Stack<IsoFileSystemEntryDirectory>();

        stack.Push(sys.RootDirectory);

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

        await Task.CompletedTask;
    }

    private static async Task StartListTracks(string source)
    {
        using var workspace = Workspace.TryOpen(source);

        var disc = workspace.GetDisc();

        foreach (var track in disc.Tracks)
        {
            Console.WriteLine($"{nameof(track.Index)}: " +
                              $"{track.Index,2}, " +
                              $"{nameof(track.Position)}: " +
                              $"{track.Position,6}, " +
                              $"{nameof(track.Length)}: " +
                              $"{track.Length,6}, " +
                              $"{nameof(track.Audio)}: " +
                              $"{track.Audio,5}");
        }

        await Task.CompletedTask;
    }
}

internal static partial class Program
{
    private static Command BuildRead()
    {
        var command = new Command("read", "Read mode.")
        {
            BuildReadSystem(),
            BuildReadTracks()
        };

        return command;
    }

    private static Command BuildReadSystem()
    {
        var target = new Argument<string>(
            "target",
            "File to read from file system."
        );

        var output = new Argument<string>(
            "output",
            "Directory to write read file to."
        );

        var cooked = new Argument<bool>(
            "cooked",
            "Read as user data or in RAW mode."
        );

        cooked.SetDefaultValue(true); // BUG prevents crash when it's omitted

        var command = new Command("system", "Reads a file from file system.")
        {
            Source, target, output, cooked
        };

        command.SetHandler(StartReadSystem, Source, target, output, cooked);

        return command;
    }

    private static Command BuildReadTracks()
    {
        var number = new Argument<int>(
            "number",
            "Track to read from disc image."
        );

        var output = new Argument<string>(
            "output",
            "Directory to write read track to."
        );

        var command = new Command("tracks", "Reads a track from disc image.")
        {
            Source, number, output
        };

        command.SetHandler(StartReadTracks, Source, number, output);

        return command;
    }

    private static async Task StartReadSystem(string source, string target, string output, bool cooked)
    {
        using var workspace = Workspace.TryOpen(source);

        var disc = workspace.GetDisc();

        var sys = workspace.GetSystem();

        if (sys.TryFindFile(target, out var file) == false)
        {
            throw new InvalidOperationException($"File could not be found in file system: '{target}'.");
        }

        CreateDirectory(output);

        var path = Path.GetFullPath(Path.Combine(output, file.FileName));

        await using var stream = File.Create(path);

        try
        {
            if (cooked)
            {
                await disc.ReadFileUserAsync(file, stream, Progress);
            }
            else
            {
                await disc.ReadFileRawAsync(file, stream, Progress);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"File could not be read from file system: '{target}'.", e);
        }
    }

    private static async Task StartReadTracks(string source, int number, string output)
    {
        using var workspace = Workspace.TryOpen(source);

        var disc = workspace.GetDisc();

        var track = disc.Tracks.FirstOrDefault(s => s.Index == number) ??
                    throw new InvalidOperationException("Track number is invalid.");

        CreateDirectory(output);

        var percent = 0.0d;

        await using var stream = File.Create(Path.Combine(output, Path.ChangeExtension($"Track {number}", track.Audio ? "wav" : "bin")));

        var sectorLength = track.Sector.Length;

        var trackLength = track.Length;

        if (track.Audio)
        {
            var length = trackLength * sectorLength;

            await stream.WriteStringAsciiAsync("RIFF");
            await stream.WriteAsync((uint)(length - 8));
            await stream.WriteStringAsciiAsync("WAVE");

            await stream.WriteStringAsciiAsync("fmt ");
            await stream.WriteAsync((uint)16);
            await stream.WriteAsync((ushort)1);    // wFormatTag
            await stream.WriteAsync((ushort)2);    // wChannels
            await stream.WriteAsync((uint)44100);  // dwSamplesPerSec
            await stream.WriteAsync((uint)176400); // dwAvgBytesPerSEc
            await stream.WriteAsync((ushort)2);    // wBlockAlign
            await stream.WriteAsync((ushort)16);   // wBitsPerSample

            await stream.WriteStringAsciiAsync("data");
            await stream.WriteAsync((uint)(length - 44));
        }

        using var buffer = new SharedBuffer<byte>(sectorLength);

        for (var i = 0; i < trackLength; i++)
        {
            var sector = await track.ReadSectorAsync(track.Position + i);

            sector.GetData().CopyTo(buffer.Span);

            await stream.WriteAsync(buffer.Memory);

            Progress.Update(ref percent, i, trackLength);

            Progress.Report(percent);
        }
    }
}