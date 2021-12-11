using System;
using FluentAssertions;
using NUnit.Framework;
using Wslhub.Sdk.Strategies.Api;
using Wslhub.Sdk.Strategies.NativeMethods;

namespace Wslhub.Sdk.Tests.IntegrationsTests
{
    [TestFixture]
    public class ComBasedApiTests
    {
        private IWslApi _api;

        [SetUp]
        public void Setup()
        {
            _api = new ComBasedWslApi(new Win32NativeMethods());

            if (!_api.IsWslSupported(out var reason))
            {
                throw new PlatformNotSupportedException(reason);
            }
        }

        [Test]
        public void Test_SystemAssertion()
        {
            var isSupported = _api.IsWslSupported();

            isSupported.Should().BeTrue();
        }

        [Test]
        public void Test_GetDistroListFromRegistryTest()
        {
            var distros = _api.GetDistroList();

            distros.Should().NotBeNull().And.HaveCountGreaterThan(0);
        }


        [Test]
        public void Test_GetDefaultDistro()
        {
            var defaultDistro = _api.GetDefaultDistro();

            defaultDistro.Should().NotBeNull();
        }
    }
}
