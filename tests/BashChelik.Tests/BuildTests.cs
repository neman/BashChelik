using System.Reflection;
using System.Runtime.Versioning;
using BashChelik;
using Xunit;

namespace BashChelik.Tests;

public class BuildTests
{
    [Fact]
    public void Library_targets_are_net11_and_netstandard20()
    {
        var target = typeof(DocumentReadFailure).Assembly
            .GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkName;

#if NETFRAMEWORK
        Assert.Equal(".NETStandard,Version=v2.0", target);
#else
        Assert.Equal(".NETCoreApp,Version=v11.0", target);
#endif
    }
}
