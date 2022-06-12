using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class CommandExecutionOptionsTests
{
    [Test]
    public void Constructor_ShouldEqualKnownValuesTests()
    {
        var ceo = new CommandExecutionOptions()
        {
            FailOnNegativeExitCode = true,
            StdErrDataProcessingMode = DataProcessingMode.External,
            StdInDataProcessingMode = DataProcessingMode.Binary,
            StdoutDataProcessingMode = DataProcessingMode.String,
            StderrEncoding = Encoding.ASCII,
            StdinEncoding = Encoding.Default,
            StdoutEncoding = Encoding.Latin1
        };

        ceo.FailOnNegativeExitCode.Should().BeTrue();
        ceo.StdErrDataProcessingMode.Should().Be(DataProcessingMode.External);
        ceo.StdInDataProcessingMode.Should().Be(DataProcessingMode.Binary);
        ceo.StdoutDataProcessingMode.Should().Be(DataProcessingMode.String);
        ceo.StderrEncoding.Should().BeSameAs(Encoding.ASCII);
        ceo.StdinEncoding.Should().BeSameAs(Encoding.Default);
        ceo.StdoutEncoding.Should().BeSameAs(Encoding.Latin1);
    }
}
