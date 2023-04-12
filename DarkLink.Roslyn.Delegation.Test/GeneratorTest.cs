using System;

namespace DarkLink.Roslyn.Delegation.Test
{
    [TestClass]
    public class GeneratorTest : VerifySourceGenerator
    {
        [TestMethod]
        public async Task Empty()
        {
            var source = string.Empty;

            await Verify(source);
        }
    }
}