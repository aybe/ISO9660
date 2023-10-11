﻿using JetBrains.Annotations;
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
            TestContext.WriteLine(IndentValueLocal.Value);
        }

        TestContext.WriteLine(value);
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

            Sender.IndentLevelLocal.Value += Indent;
        }

        public void Dispose()
        {
            Sender.IndentLevelLocal.Value -= Indent;
        }
    }
}