namespace DarkLink.Roslyn.Delegation.Test;

[TestClass]
public class PropertyDelegationGeneratorTest : VerifySourceGenerator<PropertyDelegationGenerator>
{
    [TestMethod]
    public async Task Empty()
    {
        var source = string.Empty;

        await Verify(source);
    }
}
