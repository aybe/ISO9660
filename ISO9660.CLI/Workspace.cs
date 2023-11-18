using ISO9660.Logical;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.CLI;

internal sealed class Workspace : Disposable
{
    private Workspace(Disc? disc, IsoFileSystem? system)
    {
        Disc   = disc;
        System = system;
    }

    private Disc? Disc { get; }

    private IsoFileSystem? System { get; }

    protected override void DisposeManaged()
    {
        System?.Dispose();

        Disc?.Dispose();
    }

    public static Workspace TryOpen(string path)
    {
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Image file could not be found: '{path}'.");
        }

        var extension = Path.GetExtension(path).ToLowerInvariant();

        Disc disc;

        try
        {
            disc = extension switch
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

        IsoFileSystem system;

        try
        {
            system = IsoFileSystem.Read(disc);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("File system could not be read from disc.", e);
        }

        return new Workspace(disc, system);
    }

    public Disc GetDisc()
    {
        return Disc ?? 
               throw new InvalidOperationException("Disc could not be read.");
    }

    public IsoFileSystem GetSystem()
    {
        return System ?? 
               throw new InvalidOperationException("File system could not be read.");
    }
}