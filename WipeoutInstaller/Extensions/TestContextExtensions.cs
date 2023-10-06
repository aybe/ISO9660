namespace WipeoutInstaller.Extensions;

public static class TestContextExtensions
{
    public static void WriteLine(this TestContext context, object? value = null)
    {
        context.WriteLine(value?.ToString());
    }
}