namespace ISO9660.Tests.JSON;

public static class TypeExtensions
{
    public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
    {
        while (type != null)
        {
            yield return type;
            type = type.BaseType;
        }
    }
}