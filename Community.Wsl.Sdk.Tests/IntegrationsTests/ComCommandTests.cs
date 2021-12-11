using FluentAssertions;
using NUnit.Framework;
using Wslhub.Sdk.Strategies.Api;
using Wslhub.Sdk.Strategies.Command;
using Wslhub.Sdk.Strategies.NativeMethods;

namespace Wslhub.Sdk.Tests.IntegrationsTests
{
    internal class ComCommandTests
    {
        private string _distroName;

        [SetUp]
        public void Setup()
        {
            IWslApi api = new ComBasedWslApi(new Win32NativeMethods());
            _distroName = api.GetDefaultDistro()!.Value.DistroName;
        }

        [Test]
        public void Test_expect_stdout_to_equal_constant()
        {
            var cmd = new ComCommand(
                _distroName,
                "echo -n test",
                new CommandExecutionOptions()
                {
                    StdoutDataProcessingMode = DataProcessingMode.String,
                    FailOnNegativeExitCode = false
                }
            );

            cmd.Start();
            var result = cmd.WaitAndGetResults();

            result.Stdout.Should().BeEquivalentTo("test");
            result.StdoutData.Should().BeNull();

            result.Stderr.Should().BeNull();
            result.StderrData.Should().BeNull();
        }

        [Test]
        public void Test_expect_stderr_to_equal_constant()
        {
            var cmd = new ComCommand(
                _distroName,
                "echo -n test 1>&2",
                new CommandExecutionOptions()
                {
                    StdErrDataProcessingMode = DataProcessingMode.String,
                    FailOnNegativeExitCode = false
                }
            );

            cmd.Start();
            var result = cmd.WaitAndGetResults();

            result.Stderr.Should().BeEquivalentTo("test");
            result.StderrData.Should().BeNull();

            result.Stdout.Should().BeNull();
            result.StdoutData.Should().BeNull();
        }

        [Test]
        public void Test_expect_stdout_to_equal_stdin()
        {
            var cmd = new ComCommand(
                _distroName,
                "/tmp/err.sh",
                new CommandExecutionOptions()
                {
                    StdoutDataProcessingMode = DataProcessingMode.String,
                    StdErrDataProcessingMode = DataProcessingMode.String,
                    StdInDataProcessingMode = DataProcessingMode.External,
                    FailOnNegativeExitCode = false
                }
            );

            var pipes = cmd.Start();
            // cmd.HasExited.Should().BeFalse();

            // Thread.Sleep(1000);

            // cmd.HasExited.Should().BeFalse();

            pipes.StandardInput.Write("testtest");

            var result = cmd.WaitAndGetResults();

            result.Stdout.Should().BeEquivalentTo("test");
            result.StdoutData.Should().BeNull();

            result.Stderr.Should().BeNull();
            result.StderrData.Should().BeNull();
        }
    }
}
