using System;
using Microsoft.Win32;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32RegistryKey : IRegistryKey
{
    private readonly RegistryKey _registryKey;

    public Win32RegistryKey(RegistryKey registryKey)
    {
        _registryKey = registryKey;
    }

    public T? GetValue<T>(string name)
    {
        return GetValue<T>(name, default(T));
    }

    public T? GetValue<T>(string name, T? defaultValue)
    {
        object value = _registryKey.GetValue(name);

        if (typeof(T) == typeof(string))
        {
            return (T)value ?? defaultValue;
        }
        else if (typeof(T) == typeof(Guid))
        {
            string? strValue = GetValue<string>(name);

            if (string.IsNullOrEmpty(strValue))
            {
                return defaultValue;
            }

            return (T)(object)Guid.Parse(strValue);
        }
        else
        {
            throw new Exception("Unsupported type " + typeof(T).FullName);
        }
    }

    public string[] GetSubKeyNames()
    {
        return _registryKey.GetSubKeyNames();
    }

    public IRegistryKey OpenSubKey(string subKey)
    {
        return new Win32RegistryKey(
            _registryKey.OpenSubKey(subKey, false)
                ?? throw new Exception("There is no sub key " + subKey)
        );
    }

    public void Dispose()
    {
        _registryKey.Dispose();
    }
}
