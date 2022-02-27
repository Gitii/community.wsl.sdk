using System;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.IntegrationsTests;

[TestFixture(Category = "Integration")]
internal class ManagedCommandTests
{
    private string _distroName = String.Empty;

    [SetUp]
    public void Setup()
    {
        IWslApi api = new WslApi();
        _distroName = api.GetDefaultDistro()!.Value.DistroName;
    }

    [Test]
    public void Test_expect_stdout_to_equal_constant()
    {
        var cmd = new Command(
            _distroName,
            "echo",
            new string[] { "-n", "test" },
            new CommandExecutionOptions() { StdoutDataProcessingMode = DataProcessingMode.String }
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
        var cmd = new Command(
            _distroName,
            "echo",
            new string[] { "-n", "test", "1>&2" },
            new CommandExecutionOptions() { StdErrDataProcessingMode = DataProcessingMode.String },
            shellExecute: true
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
        var cmd = new Command(
            _distroName,
            "read",
            new string[] { "-n", "4" },
            new CommandExecutionOptions()
            {
                StdoutDataProcessingMode = DataProcessingMode.String,
                StdErrDataProcessingMode = DataProcessingMode.String,
                StdInDataProcessingMode = DataProcessingMode.External
            },
            shellExecute: true
        );

        var pipes = cmd.Start();

        pipes.StandardInput.Write("test");

        var result = cmd.WaitAndGetResults();

        result.Stdout.Should().BeEquivalentTo("test");
        result.StdoutData.Should().BeNull();

        result.Stderr.Should().BeEmpty();
        result.StderrData.Should().BeNull();
    }

    [Test]
    public async Task Test_async_waitAsync()
    {
        var cmd = new Command(
            _distroName,
            "echo",
            new string[] { "-n", "test" },
            new CommandExecutionOptions() { StdoutDataProcessingMode = DataProcessingMode.String }
        );

        cmd.Start();
        var result = await cmd.WaitAndGetResultsAsync().ConfigureAwait(false);

        result.Stdout.Should().BeEquivalentTo("test");
        result.StdoutData.Should().BeNull();

        result.Stderr.Should().BeNull();
        result.StderrData.Should().BeNull();
    }

    [Test]
    public void Test_exit_code()
    {
        var cmd = new Command(
            _distroName,
            "this_command_doesnt_exit",
            new[] { "-n", "test" },
            new CommandExecutionOptions() { FailOnNegativeExitCode = false }
        );

        cmd.Start();
        var result = cmd.WaitAndGetResults();

        result.ExitCode.Should().NotBe(0);
    }

    [Test]
    public async Task Test_exit_code_asyncAsync()
    {
        var cmd = new Command(
            _distroName,
            "this_command_doesnt_exit",
            new[] { "-n", "test" },
            new CommandExecutionOptions() { FailOnNegativeExitCode = false }
        );

        cmd.Start();
        var result = await cmd.WaitAndGetResultsAsync().ConfigureAwait(false);

        result.ExitCode.Should().NotBe(0);
    }
}
