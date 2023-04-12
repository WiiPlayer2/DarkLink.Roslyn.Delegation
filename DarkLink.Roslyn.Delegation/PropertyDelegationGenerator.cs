using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DarkLink.RoslynHelpers;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

[Generator]
public class PropertyDelegationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);

        var propertyDelegations = Delegated.Find(
                context.SyntaxProvider,
                FilterNodes,
                TransformNodes)
            .Where(x => x is not null)
            .Select((x, _) => x!)
            .Collect()
            .SelectMany((x, _) => x
                .GroupBy(x => x.TargetField.ContainingType, SymbolEqualityComparer.Default)
                .Select(x => new PropertyDelegationsType((INamedTypeSymbol) x.Key!, x.ToList()))
                .ToList());

        context.RegisterImplementationSourceOutput(propertyDelegations, GeneratePropertyDelegation);
    }

    private bool FilterNodes(SyntaxNode arg1, CancellationToken arg2) => true;

    private void GeneratePropertyDelegation(SourceProductionContext context, PropertyDelegationsType delegationsType)
    {
        using var codeBuilder = new StringWriter();
        var delegationWriter = new PropertyDelegationWriter(codeBuilder, delegationsType);
        delegationWriter.Write();
        var source = codeBuilder.ToString();

        context.AddSource($"{delegationsType.TargetType}.g.cs", source);
    }

    private void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        Delegated.AddTo(context);
    }

    private PropertyDelegation? TransformNodes(GeneratorAttributeSyntaxContext context, IReadOnlyList<Delegated> attributes, CancellationToken arg3)
    {
        if (context.TargetSymbol is not IFieldSymbol fieldSymbol)
            return default;

        var getMembers = fieldSymbol.Type.GetMembers("Get").OfType<IMethodSymbol>().ToList();
        if (!getMembers.Any())
            return default;

        var getMember = getMembers.First();
        if (getMember.ReturnsVoid || getMember.ReturnType is not INamedTypeSymbol returnType)
            return default;

        var attribute = attributes.First();
        return new PropertyDelegation(
            attribute,
            fieldSymbol,
            attribute.PropertyName ?? fieldSymbol.Name.Capitalize(),
            returnType);
    }
}

internal static class Extensions
{
    public static string Capitalize(this string str)
        => string.IsNullOrEmpty(str)
            ? string.Empty
            : $"{char.ToUpperInvariant(str[0])}{str[1..str.Length]}";
}

internal record PropertyDelegationsType(INamedTypeSymbol TargetType, IReadOnlyList<PropertyDelegation> Delegations);

internal record PropertyDelegation(Delegated Data, IFieldSymbol TargetField, string Name, INamedTypeSymbol Type);

[GenerateAttribute(AttributeTargets.Field, AllowMultiple = false)]
internal partial record Delegated(string? PropertyName = default, bool GetOnly = false);
