using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

internal class PropertyDelegationWriter
{
    private readonly PropertyDelegationsType delegationsType;

    private readonly IndentedTextWriter writer;

    public PropertyDelegationWriter(TextWriter writer, PropertyDelegationsType delegationsType)
    {
        this.delegationsType = delegationsType;
        this.writer = new IndentedTextWriter(writer);
    }

    public void Write()
    {
        if (delegationsType.TargetType.ContainingNamespace.IsGlobalNamespace)
            WriteInsideType(delegationsType.TargetType, WriteContent);
        else
            WriteInsideNamespace(delegationsType.TargetType.ContainingNamespace, () => WriteInsideType(delegationsType.TargetType, WriteContent));
    }

    private void WriteContent()
    {
        foreach (var delegation in delegationsType.Delegations)
            WriteDelegation(delegation);
    }

    private void WriteDelegation(PropertyDelegation delegation)
    {
        var propertyInfoFieldName = $"____{delegation.Name}_PropertyInfo";

        writer.WriteLine($"private static readonly System.Reflection.PropertyInfo {propertyInfoFieldName} = typeof({delegationsType.TargetType}).GetProperty(nameof({delegation.Name}))!;");
        writer.WriteLine($"public {delegation.Type} {delegation.Name}");
        writer.WriteLine("{");
        writer.Indent++;
        writer.WriteLine($"get => {delegation.TargetField.Name}.Get(this, {propertyInfoFieldName});");
        if (!delegation.Data.GetOnly)
            writer.WriteLine($"set => {delegation.TargetField.Name}.Set(this, {propertyInfoFieldName}, value);");
        writer.Indent--;
        writer.WriteLine("}");
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
}
