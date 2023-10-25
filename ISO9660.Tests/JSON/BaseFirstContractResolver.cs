using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ISO9660.Tests.JSON;

public class BaseFirstContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization)
            ?.OrderBy(p => p.DeclaringType.BaseTypesAndSelf().Count()).ToList();
    }
}