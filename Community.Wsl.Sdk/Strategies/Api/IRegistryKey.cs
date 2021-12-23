using System;
using Microsoft.Win32;

namespace Community.Wsl.Sdk.Strategies.Api;

public interface IRegistryKey : IDisposable
{
    public T? GetValue<T>(string name);
    public T? GetValue<T>(string name, T? defaultValue);
    public string[] GetSubKeyNames();

    /// <summary>
    /// <inheritdoc cref="RegistryKey.OpenSubKey(string)"/>
    /// </summary>
    public IRegistryKey OpenSubKey(string subKey);
}
