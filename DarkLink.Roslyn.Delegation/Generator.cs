using System;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(PostInitialize);

            // Initialize
        }

        private void PostInitialize(IncrementalGeneratorPostInitializationContext context)
        {
            // Generate immutable code
        }
    }
}