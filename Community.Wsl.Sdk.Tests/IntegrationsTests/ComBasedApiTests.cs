using System;
using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.NativeMethods;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.IntegrationsTests
{
    [TestFixture(Category = "Integration")]
    public class ComBasedApiTests
    {
        private IWslApi _api = new ComBasedWslApi();

        [SetUp]
        public void Setup()
        {
            BaseNativeMethods nativeMethods = new Win32NativeMethods();
            _api = new ComBasedWslApi(nativeMethods);

            if (!_api.IsWslSupported(out var reason))
            {
                throw new PlatformNotSupportedException(reason);
            }
        }

        [Test]
        public void Test_IsWslSupported()
        {
            var isSupported = _api.IsWslSupported();

            isSupported.Should().BeTrue();
        }

        [Test]
        public void Test_GetDistroListFromRegistryTest()
        {
            var distros = _api.GetDistroList();

            distros.Should().NotBeNull();
        }

        [Test]
        public void Test_GetDefaultDistro()
        {
            var defaultDistro = _api.GetDefaultDistro();

            defaultDistro.Should().NotBeNull();
        }
    }
}
