using System;
using System.IO;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Commands;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class StreamDataReaderTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void CopyResultTo_ShouldHaveEmptyOutputTests(bool isStdOut)
    {
        var reader = new StreamReader(new MemoryStream(new byte[] { 1, 2 }));

        var snr = new StreamDataReader(reader);

        snr.Fetch();
        snr.Wait();

        CommandResult r = new CommandResult();

        snr.CopyResultTo(ref r, isStdOut);

        if (isStdOut)
        {
            r.Stdout.Should().BeNull();
            r.StdoutData.Should().Equal(1, 2);
            r.Stderr.Should().BeNull();
            r.StderrData.Should().BeNull();
            r.ExitCode.Should().Be(0);
        }
        else
        {
            r.Stdout.Should().BeNull();
            r.StdoutData.Should().BeNull();
            r.Stderr.Should().BeNull();
            r.StderrData.Should().Equal(1, 2);
            r.ExitCode.Should().Be(0);
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task CopyResultTo_ShouldHaveEmptyOutputTestsAsync(bool isStdOut)
    {
        var reader = new StreamReader(new MemoryStream(new byte[] { 1, 2 }));

        var snr = new StreamDataReader(reader);

        snr.Fetch();
        await snr.WaitAsync().ConfigureAwait(true);

        CommandResult r = new CommandResult();

        snr.CopyResultTo(ref r, isStdOut);

        if (isStdOut)
        {
            r.Stdout.Should().BeNull();
            r.StdoutData.Should().Equal(1, 2);
            r.Stderr.Should().BeNull();
            r.StderrData.Should().BeNull();
            r.ExitCode.Should().Be(0);
        }
        else
        {
            r.Stdout.Should().BeNull();
            r.StdoutData.Should().BeNull();
            r.Stderr.Should().BeNull();
            r.StderrData.Should().Equal(1, 2);
            r.ExitCode.Should().Be(0);
        }
    }

    [Test]
    public void Fetch_ShouldFailWhenFetchTwiceTests()
    {
        var srn = new StreamDataReader(new StreamReader(Stream.Null));
        srn.Fetch();

        var call = () => srn.Fetch();

        call.Should().Throw<ArgumentException>("Already started fetching!");
    }

    [Test]
    public void CopyResultTo_ShouldFailWhenNotFetchedTests()
    {
        var srn = new StreamDataReader(new StreamReader(Stream.Null));

        CommandResult _ = new CommandResult();
        var call = () => srn.CopyResultTo(ref _, false);

        call.Should().Throw<ArgumentException>("Data hasn't been fetched, yet!");
    }

    [Test]
    public void CopyResultTo_ShouldFailWhenNotWaitedTests()
    {
        var stream = new BlockingReadOnlyStream();
        var srn = new StreamDataReader(new StreamReader(stream));
        srn.Fetch();

        CommandResult _ = new CommandResult();
        var call = () => srn.CopyResultTo(ref _, false);

        call.Should().Throw<ArgumentException>("Fetching hasn't been finished, yet!");
    }
}
