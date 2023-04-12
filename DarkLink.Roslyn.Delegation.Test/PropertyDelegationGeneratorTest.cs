namespace DarkLink.Roslyn.Delegation.Test;

[TestClass]
public class PropertyDelegationGeneratorTest : VerifySourceGenerator<PropertyDelegationGenerator>
{
    [TestMethod]
    public async Task DelegatedInterface()
    {
        var source = @"
using System.Reflection;
using DarkLink.Roslyn.Delegation;

internal interface IPropertyDelegate<T>
{
    T Get(object? thisRef, PropertyInfo property);

    void Set(object? thisRef, PropertyInfo property, T value);
}

public partial class A
{
    [Delegated]
    private readonly IPropertyDelegate<string> stringProperty;
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
