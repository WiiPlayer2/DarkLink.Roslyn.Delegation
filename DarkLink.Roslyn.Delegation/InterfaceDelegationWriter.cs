using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

internal class InterfaceDelegationWriter
{
    private readonly InterfaceDelegation delegation;

    private readonly IndentedTextWriter writer;

    public InterfaceDelegationWriter(TextWriter writer, InterfaceDelegation delegation)
    {
        this.delegation = delegation;
        this.writer = new IndentedTextWriter(writer);
    }

    private bool IsMemberImplemented(ISymbol memberSymbol)
    {
        if (memberSymbol is not IMethodSymbol methodSymbol)
            return false;

        var implicitMembers = delegation.TargetType.GetMembers(memberSymbol.Name);
        var explicitMembers = delegation.TargetType.GetMembers($"{memberSymbol.ContainingType}.{memberSymbol.Name}");

        return explicitMembers
            .Concat(implicitMembers)
            .OfType<IMethodSymbol>()
            .Any(m => HasSameSignature(methodSymbol, m));

        bool HasSameSignature(IMethodSymbol method1, IMethodSymbol method2)
            => SymbolEqualityComparer.Default.Equals(method1.ReturnType, method2.ReturnType)
               && method1.Parameters.Select(p => p.Type)
                   .SequenceEqual(method2.Parameters.Select(p => p.Type), SymbolEqualityComparer.Default);
    }

    public void Write()
    {
        if (delegation.TargetType.ContainingNamespace.IsGlobalNamespace)
            WriteInsideType(delegation.TargetType, WriteContent);
        else
            WriteInsideNamespace(delegation.TargetType.ContainingNamespace, () => WriteInsideType(delegation.TargetType, WriteContent));
    }

    private void WriteContent()
    {
        foreach (var member in delegation.Data.InterfaceType.GetMembers())
            WriteMember(member);
    }

    private void WriteInsideNamespace(INamespaceSymbol namespaceSymbol, Action act)
    {
        writer.WriteLine($"namespace {namespaceSymbol}");
        writer.WriteLine("{");
        writer.Indent++;
        act();
        writer.Indent--;
        writer.WriteLine("}");
    }

    private void WriteInsideType(INamedTypeSymbol typeSymbol, Action act)
    {
        if (typeSymbol.ContainingType is not null)
            WriteInsideType(typeSymbol.ContainingType, WriteType);
        else
            WriteType();

        void WriteType()
        {
            writer.WriteLine($"partial class {typeSymbol.Name}");
            writer.WriteLine("{");
            writer.Indent++;
            act();
            writer.Indent--;
            writer.WriteLine("}");
        }
    }

    private void WriteMember(ISymbol memberSymbol)
    {
        if (IsMemberImplemented(memberSymbol))
            return;

        switch (memberSymbol)
        {
            case IMethodSymbol methodSymbol:
                WriteMethod(methodSymbol);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    private void WriteMethod(IMethodSymbol methodSymbol)
        => writer.WriteLine($"{methodSymbol.ReturnType} {delegation.Data.InterfaceType}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => $"{p.Type} {p.Name}"))}) => {delegation.Data.FieldName}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => p.Name))});");
}
