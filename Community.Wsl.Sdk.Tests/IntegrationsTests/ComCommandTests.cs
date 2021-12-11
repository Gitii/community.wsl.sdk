using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.Command;
using Community.Wsl.Sdk.Strategies.NativeMethods;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.IntegrationsTests
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

            AssertionExtensions.Should((string)result.Stdout).BeEquivalentTo("test");
            result.StdoutData.Should().BeNull();

            AssertionExtensions.Should((string)result.Stderr).BeNull();
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

            AssertionExtensions.Should((string)result.Stderr).BeEquivalentTo("test");
            result.StderrData.Should().BeNull();

            AssertionExtensions.Should((string)result.Stdout).BeNull();
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

            AssertionExtensions.Should((string)result.Stdout).BeEquivalentTo("test");
            result.StdoutData.Should().BeNull();

            AssertionExtensions.Should((string)result.Stderr).BeNull();
            result.StderrData.Should().BeNull();
        }
    }
}
