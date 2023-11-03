using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ISO9660.Tests.Templates;

/// <remarks>
///     <para>
///         The test methods should implement members as different parameters.
///     </para>
///     <para>
///         Otherwise, the tests are consolidated in Live Unit Testing window.
///     </para>
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class TestData
{
    public static T[] GetJsonTestData<T>(string path) where T : TestData
    {
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

        var json = File.ReadAllText(path);

        var data = JsonConvert.DeserializeObject<T[]>(json);

        return data!;
    }
}