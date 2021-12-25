﻿using System;
using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.Command;
using Community.Wsl.Sdk.Strategies.NativeMethods;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.IntegrationsTests
{
    [TestFixture(Category = "Integration")]
    internal class ComCommandTests
    {
        private string _distroName = String.Empty;

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
    }
}
