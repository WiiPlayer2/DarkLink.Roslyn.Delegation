namespace DarkLink.Roslyn.Delegation.Test;

[TestClass]
public class GeneratorTest : VerifySourceGenerator
{
    [TestMethod]
    public async Task DelegateInterfaceHierarchy()
    {
        var source = @"
using DarkLink.Roslyn.Delegation;

public interface IBaseA
{
    bool FooA(string bar);
}

public interface IBaseB
{
    int FooB(double bar);
}

public interface IBase : IBaseA, IBaseB
{
    void Foo(object bar);
}

[DelegateTo(typeof(IBase), ""b"")]
public partial class Sub : IBase
{
    private readonly IBase b;

    public Sub(IBase b)
    {
        this.b = b;
    }
}
";

        await Verify(source);
    }

    [TestMethod]
    public async Task DelegateInterfaceInNestedType()
    {
        var source = @"
using DarkLink.Roslyn.Delegation;

public interface IBase
{
    bool Foo(string bar);
}

namespace Top.Second
{
    public partial class Outer
    {
        [DelegateTo(typeof(IBase), ""b"")]
        public partial class Sub : IBase
        {
            private readonly IBase b;

            public Sub(IBase b)
            {
                this.b = b;
            }
        }
    }
}
";

        await Verify(source);
    }

    [TestMethod]
    public async Task DelegateSingleInterfaceWithMethod()
    {
        var source = @"
using DarkLink.Roslyn.Delegation;

public interface IBase
{
    bool Foo(string bar);
}

[DelegateTo(typeof(IBase), ""b"")]
public partial class Sub : IBase
{
    private readonly IBase b;

    public Sub(IBase b)
    {
        this.b = b;
    }
}
";

        await Verify(source);
    }

    [TestMethod]
    public async Task Empty()
    {
        var source = string.Empty;

        await Verify(source);
    }
}
