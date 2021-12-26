using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.Command;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.UnitTests
{
    public class ManagedCommandTests
    {
        public ManagedCommand CreateCommand(
            string distroName,
            string command,
            string[] arguments,
            CommandExecutionOptions options,
            bool asRoot = false,
            bool shellExecute = false
        )
        {
            var io = A.Fake<IIo>();
            var env = A.Fake<IEnvironment>();
            var pm = A.Fake<IProcessManager>();

            return new ManagedCommand(
                distroName,
                command,
                arguments,
                options,
                asRoot,
                shellExecute,
                env,
                io,
                pm
            );
        }

        [Test]
        public void Test_Constructor()
        {
            var cmd = CreateCommand("dn", "", Array.Empty<string>(), new CommandExecutionOptions());

            cmd.HasExited.Should().BeFalse();
            cmd.HasWaited.Should().BeFalse();
            cmd.IsDisposed.Should().BeFalse();
            cmd.IsStarted.Should().BeFalse();
        }

        [Test]
        public void Test_Start()
        {
            var distroName = "distro";
            var command = "command";
            var arguments = new string[] { "args" };
            var options = new CommandExecutionOptions();

            var io = A.Fake<IIo>();
            var env = A.Fake<IEnvironment>();
            var pm = A.Fake<IProcessManager>();

            var p = A.Fake<IProcess>();

            ProcessStartInfo actualStartInfo = new ProcessStartInfo();
            A.CallTo(() => pm.Start(A<ProcessStartInfo>._))
                .Invokes((psi) => actualStartInfo = psi.GetArgument<ProcessStartInfo>(0)!)
                .Returns(p);

            var cmd = new ManagedCommand(
                distroName,
                command,
                arguments,
                options,
                false,
                false,
                env,
                io,
                pm
            );

            var results = cmd.Start();

            results.StandardOutput.Should().Be(StreamReader.Null);
            results.StandardError.Should().Be(StreamReader.Null);
            results.StandardInput.Should().Be(StreamWriter.Null);

            A.CallTo(() => pm.Start(A<ProcessStartInfo>._)).MustHaveHappened();

            actualStartInfo.ArgumentList
                .Should()
                .BeEquivalentTo("-d", distroName, "--exec", command, arguments[0]);
            actualStartInfo.RedirectStandardOutput.Should().BeFalse();
            actualStartInfo.RedirectStandardError.Should().BeFalse();
            actualStartInfo.RedirectStandardInput.Should().BeFalse();

            actualStartInfo.CreateNoWindow.Should().BeTrue();
        }
    }
}
