using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using JetBrains.Annotations;

namespace WipeoutInstaller.Templates;

/// <remarks>
///     <para>
///         The test methods should implement members as different parameters.
///     </para>
///     <para>
///         Otherwise, the tests are consolidated in Live Unit Testing window.
///     </para>
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract record TestData
{
    public static T[] GetCsvTestData<T>(string path) where T : TestData
    {
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true, BadDataFound = null, IgnoreBlankLines = true, TrimOptions = TrimOptions.Trim
        };

        using var reader = new CsvReader(new StreamReader(File.OpenRead(path)), configuration);

        var records = reader.GetRecords<T>();

        var array = records.ToArray();

        return array;
    }
}