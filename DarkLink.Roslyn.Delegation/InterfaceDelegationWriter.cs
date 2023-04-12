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

        void WriteMember(ISymbol memberSymbol)
        {
            switch (memberSymbol)
            {
                case IMethodSymbol methodSymbol:
                    WriteMethod(methodSymbol);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
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

    private void WriteMethod(IMethodSymbol methodSymbol)
        => writer.WriteLine($"{methodSymbol.ReturnType} {delegation.Data.InterfaceType}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => $"{p.Type} {p.Name}"))}) => {delegation.Data.FieldName}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => p.Name))});");
}
