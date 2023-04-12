using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitialize);
        var interfaceDelegations = DelegateTo.Find(
                context.SyntaxProvider,
                FilterNodes,
                TransformNodes)
            .SelectMany((x, _) => x);

        context.RegisterImplementationSourceOutput(interfaceDelegations, GenerateInterfaceDelegation);
    }

    private bool FilterNodes(SyntaxNode arg1, CancellationToken arg2) => true;

    private void GenerateInterfaceDelegation(SourceProductionContext context, InterfaceDelegation delegation)
    {
        using var codeBuilder = new StringWriter();
        codeBuilder.WriteLine($"partial class {delegation.TargetType.Name} {{");

        foreach (var member in delegation.Data.InterfaceType.GetMembers())
            switch (member)
            {
                case IMethodSymbol methodSymbol:
                    codeBuilder.WriteLine($"{methodSymbol.ReturnType} {delegation.Data.InterfaceType}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => $"{p.Type} {p.Name}"))}) => {delegation.Data.FieldName}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => p.Name))});");
                    break;

                default:
                    throw new NotImplementedException();
            }

        codeBuilder.WriteLine("}");

        var source = codeBuilder.ToString();
        context.AddSource($"{delegation.TargetType}_{delegation.Data.InterfaceType}.g.cs", source);
    }

    private void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        DelegateTo.AddTo(context);
    }

    private IReadOnlyList<InterfaceDelegation> TransformNodes(GeneratorAttributeSyntaxContext context, IReadOnlyList<DelegateTo> attributes, CancellationToken arg3)
    {
        if (context.TargetSymbol is not INamedTypeSymbol targetType)
            return Array.Empty<InterfaceDelegation>();

        return attributes
            .SelectMany(x => x.InterfaceType.AllInterfaces
                .Concat(new[] {x.InterfaceType})
                .Select(i => new InterfaceDelegation(targetType, x with {InterfaceType = i})))
            .ToList();
    }

    private record InterfaceDelegation(INamedTypeSymbol TargetType, DelegateTo Data);
}
