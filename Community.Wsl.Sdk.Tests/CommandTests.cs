using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Community.Wsl.Sdk.Strategies.Commands;
using Community.Wsx.Shared;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests;

public class CommandTests
{
    public Command CreateCommand(
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

        return new Command(
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
    public void Constructor_ShouldHaveDefaultValues()
    {
        var cmd = CreateCommand("dn", "", Array.Empty<string>(), new CommandExecutionOptions());

        cmd.HasExited.Should().BeFalse();
        cmd.HasWaited.Should().BeFalse();
        cmd.IsDisposed.Should().BeFalse();
        cmd.IsStarted.Should().BeFalse();
    }

    [TestCase(DataProcessingMode.Binary)]
    [TestCase(DataProcessingMode.String)]
    public void Constructor_ShouldFailWhenOptionsAreInvalid(DataProcessingMode mode)
    {
        var call = () =>
            CreateCommand(
                "dn",
                "",
                Array.Empty<string>(),
                new CommandExecutionOptions() { StdInDataProcessingMode = mode, }
            );

        call.Should().Throw<ArgumentException>("StandardInput can only be dropped or external.");
    }

    [TestCase(true, false)]
    [TestCase(false, false)]
    public void Start_ShouldCreateProcessAndStartIt(bool isRoot, bool shellExecute)
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

        var cmd = new Command(
            distroName,
            command,
            arguments,
            options,
            isRoot,
            shellExecute,
            env,
            io,
            pm
        );

        var results = cmd.Start();

        results.StandardOutput.Should().Be(StreamReader.Null);
        results.StandardError.Should().Be(StreamReader.Null);
        results.StandardInput.Should().Be(StreamWriter.Null);

        A.CallTo(() => pm.Start(A<ProcessStartInfo>._)).MustHaveHappened();

        var expectedArgs = new List<string>() { "-d", distroName };

        if (isRoot)
        {
            expectedArgs.Add("--user");
            expectedArgs.Add("root");
        }

        expectedArgs.Add(shellExecute ? "--" : "--exec");
        expectedArgs.Add(command);
        expectedArgs.AddRange(arguments);

        actualStartInfo.ArgumentList.Should().BeEquivalentTo(expectedArgs);

        actualStartInfo.RedirectStandardOutput.Should().BeFalse();
        actualStartInfo.RedirectStandardError.Should().BeFalse();
        actualStartInfo.RedirectStandardInput.Should().BeFalse();

        actualStartInfo.CreateNoWindow.Should().BeTrue();
    }

    [TestCase(DataProcessingMode.Drop, typeof(StreamNullReader))]
    [TestCase(DataProcessingMode.Binary, typeof(StreamDataReader))]
    [TestCase(DataProcessingMode.String, typeof(StreamStringReader))]
    [TestCase(DataProcessingMode.External, typeof(StreamNullReader))]
    public void Start_ShouldCreateStdoutStreams(DataProcessingMode mode, Type readerType)
    {
        var distroName = "distro";
        var command = "command";
        var arguments = new string[] { "args" };
        var options = new CommandExecutionOptions() { StdoutDataProcessingMode = mode };

        var io = A.Fake<IIo>();
        var env = A.Fake<IEnvironment>();
        var pm = A.Fake<IProcessManager>();

        var p = A.Fake<IProcess>();
        A.CallTo(() => p.StandardOutput).Returns(new StreamReader(Stream.Null));

        ProcessStartInfo actualStartInfo = new ProcessStartInfo();
        A.CallTo(() => pm.Start(A<ProcessStartInfo>._))
            .Invokes((psi) => actualStartInfo = psi.GetArgument<ProcessStartInfo>(0)!)
            .Returns(p);

        var cmd = new Command(distroName, command, arguments, options, false, false, env, io, pm);

        var results = cmd.Start();

        if (mode == DataProcessingMode.Drop)
        {
            actualStartInfo.RedirectStandardOutput.Should().BeFalse();
            results.StandardOutput.Should().Be(StreamReader.Null);
        }
        else
        {
            actualStartInfo.RedirectStandardOutput.Should().BeTrue();
            results.StandardOutput.Should().NotBe(StreamReader.Null);
        }

        results.StandardError.Should().Be(StreamReader.Null);
        results.StandardInput.Should().Be(StreamWriter.Null);
        actualStartInfo.RedirectStandardError.Should().BeFalse();
        actualStartInfo.RedirectStandardInput.Should().BeFalse();

        cmd.StdoutReader.Should().BeOfType(readerType);
    }

    [TestCase(DataProcessingMode.Drop, typeof(StreamNullReader))]
    [TestCase(DataProcessingMode.Binary, typeof(StreamDataReader))]
    [TestCase(DataProcessingMode.String, typeof(StreamStringReader))]
    [TestCase(DataProcessingMode.External, typeof(StreamNullReader))]
    public void Start_ShouldCreateStderrStreams(DataProcessingMode mode, Type readerType)
    {
        var distroName = "distro";
        var command = "command";
        var arguments = new string[] { "args" };
        var options = new CommandExecutionOptions() { StdErrDataProcessingMode = mode };

        var io = A.Fake<IIo>();
        var env = A.Fake<IEnvironment>();
        var pm = A.Fake<IProcessManager>();

        var p = A.Fake<IProcess>();

        A.CallTo(() => p.StandardError).Returns(new StreamReader(Stream.Null));

        ProcessStartInfo actualStartInfo = new ProcessStartInfo();
        A.CallTo(() => pm.Start(A<ProcessStartInfo>._))
            .Invokes((psi) => actualStartInfo = psi.GetArgument<ProcessStartInfo>(0)!)
            .Returns(p);

        var cmd = new Command(distroName, command, arguments, options, false, false, env, io, pm);

        var results = cmd.Start();

        if (mode == DataProcessingMode.Drop)
        {
            actualStartInfo.RedirectStandardError.Should().BeFalse();
            results.StandardError.Should().Be(StreamReader.Null);
        }
        else
        {
            actualStartInfo.RedirectStandardError.Should().BeTrue();
            results.StandardError.Should().NotBe(StreamReader.Null);
        }

        results.StandardOutput.Should().Be(StreamReader.Null);
        results.StandardInput.Should().Be(StreamWriter.Null);
        actualStartInfo.RedirectStandardOutput.Should().BeFalse();
        actualStartInfo.RedirectStandardInput.Should().BeFalse();

        cmd.StderrReader.Should().BeOfType(readerType);
    }

    [TestCase(DataProcessingMode.Drop, typeof(StreamNullReader))]
    [TestCase(DataProcessingMode.External, typeof(StreamNullReader))]
    public void Start_ShouldCreateStdInStreams(DataProcessingMode mode, Type readerType)
    {
        var distroName = "distro";
        var command = "command";
        var arguments = new string[] { "args" };
        var options = new CommandExecutionOptions() { StdInDataProcessingMode = mode };

        var io = A.Fake<IIo>();
        var env = A.Fake<IEnvironment>();
        var pm = A.Fake<IProcessManager>();

        var p = A.Fake<IProcess>();

        A.CallTo(() => p.StandardInput).Returns(new StreamWriter(Stream.Null));

        ProcessStartInfo actualStartInfo = new ProcessStartInfo();
        A.CallTo(() => pm.Start(A<ProcessStartInfo>._))
            .Invokes((psi) => actualStartInfo = psi.GetArgument<ProcessStartInfo>(0)!)
            .Returns(p);

        var cmd = new Command(distroName, command, arguments, options, false, false, env, io, pm);

        var results = cmd.Start();

        if (mode == DataProcessingMode.Drop)
        {
            actualStartInfo.RedirectStandardInput.Should().BeFalse();
            results.StandardInput.Should().Be(StreamWriter.Null);
        }
        else
        {
            actualStartInfo.RedirectStandardInput.Should().BeTrue();
            results.StandardInput.Should().NotBe(StreamWriter.Null);
        }

        results.StandardOutput.Should().Be(StreamReader.Null);
        results.StandardError.Should().Be(StreamReader.Null);
        actualStartInfo.RedirectStandardOutput.Should().BeFalse();
        actualStartInfo.RedirectStandardError.Should().BeFalse();
    }

    [Test]
    public void Start_ShouldFailWhenStartedTwice()
    {
        var distroName = "distro";
        var command = "command";
        var arguments = new string[] { "args" };
        var options = new CommandExecutionOptions();

        var io = A.Fake<IIo>();
        var env = A.Fake<IEnvironment>();
        var pm = A.Fake<IProcessManager>();

        var cmd = new Command(distroName, command, arguments, options, false, false, env, io, pm);

        cmd.Start();

        var call = () => cmd.Start();

        cmd.IsStarted.Should().BeTrue();

        call.Should().Throw<Exception>("Command has already been started!");
    }
}
