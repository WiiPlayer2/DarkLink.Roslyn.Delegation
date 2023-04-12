//HintName: DarkLink.Roslyn.Delegation.Delegated.g.cs
using System;

namespace DarkLink.Roslyn.Delegation
{
    [AttributeUsage((AttributeTargets)256, AllowMultiple = false, Inherited = true)]
    internal class Delegated : Attribute
    {
        public Delegated() { }
        public string? PropertyName { get; set; }
    }
}
