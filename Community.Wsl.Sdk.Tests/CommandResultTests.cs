using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class CommandResultTests
{
    [Test]
    public void Constructor_ShouldEqualKnownValuesTests()
    {
        var cr = new CommandResult()
        {
            ExitCode = 0,
            Stderr = "a",
            StderrData = new byte[] { 1 },
            Stdout = "b",
            StdoutData = new byte[] { 2 }
        };

        cr.ExitCode.Should().Be(0);
        cr.Stderr.Should().Be("a");
        cr.StderrData.Should().Equal(1);
        cr.Stdout.Should().Be("b");
        cr.StdoutData.Should().Equal(2);
    }
}
