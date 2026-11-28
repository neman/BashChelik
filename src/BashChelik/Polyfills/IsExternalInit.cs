#if !NET
namespace System.Runtime.CompilerServices
{
    /// <summary>Lets <c>init</c> accessors and records compile on .NET Standard 2.0.</summary>
    internal static class IsExternalInit
    {
    }
}
#endif
