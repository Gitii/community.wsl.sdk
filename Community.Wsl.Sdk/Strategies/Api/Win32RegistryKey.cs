using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32RegistryKey : IRegistryKey
{
    private readonly RegistryKey _registryKey;

    public Win32RegistryKey(RegistryKey registryKey)
    {
        _registryKey = registryKey;
    }

    public T GetValue<T>(string name)
    {
        var value = GetValue<T>(name, default(T)!);
        if (value == null || value.Equals(default(T)))
        {
            throw new KeyNotFoundException($"The registry key {name} doesn't exist!");
        }

        return value;
    }

    public T GetValue<T>(string name, T defaultValue)
    {
        object? value = _registryKey.GetValue(name);

        if (value == null)
        {
            return defaultValue;
        }
        else if (typeof(T) == value.GetType())
        {
            return (T)value;
        }
        else if (typeof(T) == typeof(Guid))
        {
            string strValue = GetValue<string>(name);

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