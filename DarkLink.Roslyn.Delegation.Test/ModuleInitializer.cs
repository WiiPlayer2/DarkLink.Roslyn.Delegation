using System.Runtime.CompilerServices;

namespace DarkLink.Roslyn.Delegation.Test
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() => VerifySourceGenerators.Enable();
    }
}