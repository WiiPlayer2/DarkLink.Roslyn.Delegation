using System;
using System.Linq;
using System.Text;
using DarkLink.RoslynHelpers;
using DarkLink.RoslynHelpers.AttributeGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DarkLink.Roslyn.Delegation;

[Generator]
public class PropertyDelegationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);
    }

    private void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        Delegated.AddTo(context);

        var assembly = typeof(Generator).Assembly;
        var injectedCodeResources = assembly.GetManifestResourceNames()
            .Where(name => name.Contains("InjectedCode"));

        foreach (var resource in injectedCodeResources)
        {
            using var stream = assembly.GetManifestResourceStream(resource)!;
            context.AddSource(resource, SourceText.From(stream, new UTF8Encoding(false), canBeEmbedded: true));
        }
    }
}

[GenerateAttribute(AttributeTargets.Field, AllowMultiple = false)]
internal partial record Delegated(string? PropertyName = default);
