using System.Diagnostics.CodeAnalysis;
using System.Text;
using ISO9660.Tests.Extensions;
using ISO9660.Tests.FileSystem.Experimental.RockRidge;
using ISO9660.Tests.FileSystem.Experimental.SystemUseSharingProtocol;

namespace ISO9660.Tests.FileSystem.Experimental;

public sealed class RockRidgeParser
{
    [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
    [SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Code coverage.")]
    public static bool TryRead(DirectoryRecord record)
    {
        using var reader = new BinaryReader(new MemoryStream(record.SystemUse), Encoding.ASCII);

        var entries = new List<SystemUseEntry>();

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            if (!reader.TryPeek(s => BinaryReaderExtensions.ReadStringAscii(s, 2), out var result))
            {
                break;
            }

            if (result != "SP" && entries.Count == 0)
            {
                break;
            }

            SystemUseEntry? entry;

            switch (result)
            {
                case "CE":
                    entry = new ContinuationArea(reader);
                    break;
                case "PD":
                    entry = new PaddingField(reader);
                    break;
                case "SP":
                    entry = new SystemUseSharingProtocolIndicator(reader);
                    break;
                case "ST":
                    entry = new SystemUseSharingProtocolTerminator(reader);
                    break;
                case "ER":
                    entry = new ExtensionsReference(reader);
                    break;
                case "ES":
                    entry = new ExtensionSelector(reader);
                    break;
                case "PX":
                    entry = new PosixFileAttributes(reader);
                    break;
                case "PN":
                    entry = new PosixDeviceNumber(reader);
                    break;
                case "SL":
                    entry = new SymbolicLink(reader);
                    break;
                case "NM":
                    entry = new AlternateName(reader);
                    break;
                case "CL":
                    entry = new ChildLink(reader);
                    break;
                case "PL":
                    entry = new ParentLink(reader);
                    break;
                case "RE":
                    entry = new RelocatedDirectory(reader);
                    break;
                case "TF":
                    entry = new FileTimeStamp(reader);
                    break;
                case "SF":
                    entry = new SparseFileFormatFileData(reader);
                    break;
                default:
                    entry = null;
                    break;
            }

            if (entry == null)
            {
                throw new NotImplementedException(
                    $"{string.Join(",", Encoding.ASCII.GetBytes(result).Select(s => $"0x{s:X2}"))} @ {reader.BaseStream.Position}");
            }

            entries.Add(entry);

            if (entry is SystemUseSharingProtocolTerminator)
            {
                break;
            }
        }

        return false;
    }
}