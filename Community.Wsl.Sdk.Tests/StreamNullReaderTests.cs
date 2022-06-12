using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class StreamNullReaderTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void CopyResultTo_ShouldHaveEmptyOutputTests(bool isStdOut)
    {
        var snr = new StreamNullReader();

        snr.Wait();
        snr.Fetch();

        CommandResult r = new CommandResult();

        snr.CopyResultTo(ref r, isStdOut);

        r.Stdout.Should().BeNull();
        r.StdoutData.Should().BeNull();
        r.Stderr.Should().BeNull();
        r.StderrData.Should().BeNull();
        r.ExitCode.Should().Be(0);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task CopyResultTo_ShouldHaveEmptyOutputTestsAsync(bool isStdOut)
    {
        var snr = new StreamNullReader();

        await snr.WaitAsync().ConfigureAwait(false);
        snr.Fetch();

        CommandResult r = new CommandResult();

        snr.CopyResultTo(ref r, isStdOut);

        r.Stdout.Should().BeNull();
        r.StdoutData.Should().BeNull();
        r.Stderr.Should().BeNull();
        r.StderrData.Should().BeNull();
        r.ExitCode.Should().Be(0);
    }
}
