using System;
using DarkLink.RoslynHelpers;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

[GenerateAttribute(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal partial record DelegateTo(INamedTypeSymbol InterfaceType, string FieldName);
