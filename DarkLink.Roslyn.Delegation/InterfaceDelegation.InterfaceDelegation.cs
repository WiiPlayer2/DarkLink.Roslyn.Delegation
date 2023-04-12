using System;
using Microsoft.CodeAnalysis;

namespace DarkLink.Roslyn.Delegation;

internal record InterfaceDelegation(INamedTypeSymbol TargetType, DelegateTo Data);
