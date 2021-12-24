using System;
using Community.Wsl.Sdk.Strategies.Api;
using Community.Wsl.Sdk.Strategies.NativeMethods;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Community.Wsl.Sdk.Tests.UnitTests
{
    public class ComBasedApiTests
    {
        [Test]
        public void Test_InitializeSecurityModel()
        {
            var nm = A.Fake<BaseNativeMethods>();

            A.CallTo(
                    () =>
                        nm.CoInitializeSecurity(
                            IntPtr.Zero,
                            -1,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            BaseNativeMethods.RpcAuthnLevel.Default,
                            BaseNativeMethods.RpcImpLevel.Impersonate,
                            IntPtr.Zero,
                            BaseNativeMethods.EoAuthnCap.StaticCloaking,
                            IntPtr.Zero
                        )
                )
                .Returns(0);

            var api = new ComBasedWslApi(nm);

            var call = () => api.InitializeSecurityModel();

            call.Should().NotThrow();

            A.CallTo(
                    () =>
                        nm.CoInitializeSecurity(
                            IntPtr.Zero,
                            -1,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            BaseNativeMethods.RpcAuthnLevel.Default,
                            BaseNativeMethods.RpcImpLevel.Impersonate,
                            IntPtr.Zero,
                            BaseNativeMethods.EoAuthnCap.StaticCloaking,
                            IntPtr.Zero
                        )
                )
                .MustHaveHappened();
        }
    }
}
