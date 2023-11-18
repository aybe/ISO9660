using ISO9660.Logical;
using ISO9660.Physical;
using Whatever.Extensions;

namespace ISO9660.CLI;

internal sealed class Workspace : Disposable
{
    private Workspace()
    {
    }

    private Disc? Disc { get; set; }

    private IsoFileSystem? System { get; set; }

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