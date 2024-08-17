using FluentAssertions;
using NUnit.Framework;
using System;
using Community.Wsx.Shared;
using FakeItEasy;
using Microsoft.VisualBasic;

namespace Community.Wsl.Sdk.Tests;

public class WslApiTests
{
    [Test]
    public void GetDistributionList_ShouldSkipNonGuidSubKeys()
    {
        var currentUser = A.Fake<IRegistryKey>();
        var lxssKey = A.Fake<IRegistryKey>();

        var validSubKey = A.Fake<IRegistryKey>();
        var validSubKeyGuid = Guid.NewGuid().ToString();

        var invalidSubKeyGuid = "invalid";

        var reg = A.Fake<IRegistry>();

        var randomString = "string";

        A.CallTo(() => reg.GetCurrentUser()).Returns(currentUser);
        A.CallTo(() => currentUser.OpenSubKey(A<string>.Ignored)).Returns(lxssKey);
        A.CallTo(() => lxssKey.GetSubKeyNames()).Returns([validSubKeyGuid, invalidSubKeyGuid]);
        A.CallTo(() => lxssKey.OpenSubKey(validSubKeyGuid)).Returns(validSubKey);
        
        A.CallTo(() => validSubKey.GetValue<string>(A<string>.Ignored)).Returns(randomString);
        A.CallTo(() => validSubKey.GetValue<int>(A<string>.Ignored)).Returns(0);
        A.CallTo(() => validSubKey.GetValue(A<string>.Ignored, A<string>.Ignored)).Returns(randomString);

        var wsl = new WslApi(reg);

        var list = wsl.GetDistributionList();

        list.Should().HaveCount(1);

        list[0].Should().BeEquivalentTo(new DistroInfo()
        {
            DistroId = Guid.Parse(validSubKeyGuid),
            DistroName = randomString,
            KernelCommandLine = [randomString],
            IsDefault = false,
            WslVersion = 0,
            DistroFlags = 0,
            DefaultUid = 0,
            DefaultEnvironmentVariables = []
        }, (options) => options.ComparingByMembers<DistroInfo>().Excluding((info => info.BasePath)));
    }
}
