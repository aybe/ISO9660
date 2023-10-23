namespace ISO9660.Tests.Extensions;

public static class TestContextExtensions
{
    public static void WriteLine(this TestContext context, object? value = null)
    {
        context.WriteLine(value?.ToString());
    }
}