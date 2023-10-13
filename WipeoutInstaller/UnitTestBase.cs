using System.Linq.Expressions;
using JetBrains.Annotations;
using WipeoutInstaller.Extensions;

namespace WipeoutInstaller;

public abstract class UnitTestBase
{
    [PublicAPI]
    public TestContext TestContext { get; set; } = null!;

    private ThreadLocal<uint> IndentLevelLocal { get; } = new(() => 0);

    private ThreadLocal<string?> IndentValueLocal { get; } = new(() => "\t");

    protected uint IndentLevel
    {
        get => IndentLevelLocal.Value;
        set => IndentLevelLocal.Value = value;
    }

    protected string? IndentValue
    {
        get => IndentValueLocal.Value;
        set => IndentValueLocal.Value = value;
    }

    protected void WriteLine(object? value = null)
    {
        for (var i = 0; i < IndentLevelLocal.Value; i++)
        {
            TestContext.Write(IndentValueLocal.Value);
        }

        TestContext.WriteLine(value);
    }

    protected void WriteLine<T>(Expression<Func<T>> expression) // T avoids Convert(...) expression
    {
        if (expression.Body is not MemberExpression me)
        {
            throw new ArgumentOutOfRangeException(nameof(expression), expression, null);
        }

        var compile = expression.Compile();

        var value = compile();

        var message = $"{me.Member.Name}: {value}";

        WriteLine(message);
    }

    protected IDisposable Indent(uint value)
    {
        return new IndentScope(this, value);
    }

    private readonly struct IndentScope : IDisposable
    {
        private UnitTestBase Sender { get; }

        private uint Indent { get; }

        public IndentScope(UnitTestBase sender, uint indent)
        {
            Sender = sender;
            Indent = indent;

            Sender.IndentLevelLocal.Value = Indent;
        }

        public void Dispose()
        {
            Sender.IndentLevelLocal.Value = Indent;
        }
    }
}