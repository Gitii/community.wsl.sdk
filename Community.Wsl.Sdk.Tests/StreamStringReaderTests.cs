using System;
using System.IO;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class StreamStringReaderTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void CopyResultTo_ShouldHaveEmptyOutputTests(bool isStdOut)
    {
        var reader = new StreamReader(new MemoryStream(new byte[] { (byte)'a' }));

        var snr = new StreamStringReader(reader);

        snr.Fetch();
        snr.Wait();

        CommandResult r = new CommandResult();

        snr.CopyResultTo(ref r, isStdOut);

        if (isStdOut)
        {
            r.Stdout.Should().Be("a");
            r.StdoutData.Should().BeNull();
            r.Stderr.Should().BeNull();
            r.StderrData.Should().BeNull();
            r.ExitCode.Should().Be(0);
        }
        else
        {
            r.Stdout.Should().BeNull();
            r.StdoutData.Should().BeNull();
            r.Stderr.Should().Be("a");
            r.StderrData.Should().BeNull();
            r.ExitCode.Should().Be(0);
        }
    }
}
