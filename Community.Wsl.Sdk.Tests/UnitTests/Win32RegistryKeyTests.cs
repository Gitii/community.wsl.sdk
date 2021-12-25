using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Api;
using FluentAssertions;
using Microsoft.Win32;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.UnitTests
{
    /// <summary>
    /// NOTE: Actually these are integration tests because they rely on the actual windows registry.
    /// The used hives and keys are well-known and should be stable on all (valid) windows-based test runner.
    /// </summary>
    public class Win32RegistryKeyTests
    {
        [Test]
        public void Test_GetSubKeyNames()
        {
            var reg = new Win32RegistryKey(Registry.LocalMachine);

            var pubs = reg.OpenSubKey(
                "Software\\Microsoft\\Windows\\CurrentVersion\\WINEVT\\Publishers"
            );

            var values = pubs.GetSubKeyNames();

            Guid dummy;
            values.Should().NotBeEmpty().And.OnlyContain((key) => Guid.TryParse(key, out dummy));
        }

        [Test]
        public void Test_GetValue_string()
        {
            var reg = new Win32RegistryKey(Registry.CurrentUser);

            var console = reg.OpenSubKey("Console");

            console.GetValue<string>("FaceName").Should().NotBeNull();
        }

        [Test]
        public void Test_GetValue_int()
        {
            var reg = new Win32RegistryKey(Registry.CurrentUser);

            var console = reg.OpenSubKey("Console");

            console.GetValue<int>("CursorSize").Should().BeGreaterThan(0);
        }

        [Test]
        public void Test_GetValue_guid()
        {
            var reg = new Win32RegistryKey(Registry.LocalMachine);

            var crypt = reg.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");

            crypt.GetValue<Guid>("MachineGuid").Should().NotBe(Guid.Empty);
        }

        [Test]
        public void Test_GetValue_not_existing_key()
        {
            var reg = new Win32RegistryKey(Registry.CurrentUser);

            var call = () => reg.GetValue<string>("IDoNotExist");

            call.Should().Throw<KeyNotFoundException>();
        }

        [Test]
        public void Test_GetValue_default_value()
        {
            var reg = new Win32RegistryKey(Registry.CurrentUser);

            var value = reg.GetValue("IDoNotExist", "foobar");

            value.Should().BeEquivalentTo("foobar");
        }

        [Test]
        public void Test_GetValue_unsupported_type()
        {
            var reg = new Win32RegistryKey(Registry.CurrentUser);

            var console = reg.OpenSubKey("Console");

            var call = () => console.GetValue<long>("CursorSize");

            call.Should().Throw<Exception>();
        }
    }
}
