using System;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class DistroInfoTests
{
    [Test]
    public void Constructor_ShouldEqualKnownInstanceTest()
    {
        var di = new DistroInfo()
        {
            DistroId = Guid.Empty,
            BasePath = "path",
            DefaultEnvironmentVariables = Array.Empty<string>(),
            DefaultUid = 0,
            DistroFlags = DistroFlags.None,
            DistroName = "name",
            IsDefault = false,
            KernelCommandLine = Array.Empty<string>(),
            WslVersion = 2
        };

        di.DistroId.Should().Be(Guid.Empty);
        di.BasePath.Should().Be("path");
        di.DefaultEnvironmentVariables.Should().BeEmpty();
        di.DefaultUid.Should().Be(0);
        di.DistroFlags.Should().Be(DistroFlags.None);
        di.DistroName.Should().Be("name");
        di.IsDefault.Should().BeFalse();
        di.KernelCommandLine.Should().BeEmpty();
        di.WslVersion.Should().Be(2);
    }

    [Test]
    public void ToString_ShouldEqualKnownStringTests()
    {
        var di = new DistroInfo() { DistroName = "test", DistroId = Guid.NewGuid(), };

        di.ToString().Should().Be($"{di.DistroName} [{di.DistroId}]");
    }
}
