using Microsoft.Win32;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32Registry : IRegistry
{
    public IRegistryKey GetCurrentUser()
    {
        return new Win32RegistryKey(Registry.CurrentUser);
    }
}
