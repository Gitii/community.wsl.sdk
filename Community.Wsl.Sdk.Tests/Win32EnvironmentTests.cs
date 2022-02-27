using System;
using Community.Wsl.Sdk.Strategies.Api;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests
{
    public class Win32EnvironmentTests
    {
        [Test]
        public void Test_Is64BitOperatingSystem()
        {
            var env = new Win32Environment();

            env.Is64BitOperatingSystem.Should().Be(Environment.Is64BitOperatingSystem);
        }

        [Test]
        public void Test_Is64BitProcess()
        {
            var env = new Win32Environment();

            env.Is64BitProcess.Should().Be(Environment.Is64BitProcess);
        }

        [Test]
        public void Test_OSVersion()
        {
            var env = new Win32Environment();

            env.OSVersion.Should().BeEquivalentTo(Environment.OSVersion);
        }

        public static readonly Environment.SpecialFolder[] FolderNames =
            Enum.GetValues<Environment.SpecialFolder>();

        [Test]
        [TestCaseSource(nameof(FolderNames))]
        public void Test_GetFolderPath(Environment.SpecialFolder folder)
        {
            var env = new Win32Environment();

            env.GetFolderPath(folder).Should().BeEquivalentTo(Environment.GetFolderPath(folder));
        }
    }
}
