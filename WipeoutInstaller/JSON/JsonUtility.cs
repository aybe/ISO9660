using Newtonsoft.Json;

namespace ISO9660.Tests.JSON;

public static class JsonUtility
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting                 = Formatting.Indented,
        ContractResolver           = new BaseFirstContractResolver(),
        PreserveReferencesHandling = PreserveReferencesHandling.All
    };

    public static string ToJson(object? value, JsonSerializerSettings? settings = null)
    {
        var json = JsonConvert.SerializeObject(value, settings ?? Settings);

        return json;
    }
}