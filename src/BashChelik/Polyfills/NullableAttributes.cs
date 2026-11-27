#if !NET
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>The value is not null when the method returns the given result.</summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }

    /// <summary>The value may be null even when its type does not allow it.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    internal sealed class MaybeNullAttribute : Attribute
    {
    }
}
#endif
