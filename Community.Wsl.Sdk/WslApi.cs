using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Community.Wsx.Shared;

namespace Community.Wsl.Sdk;

/// <summary>
/// <inheritdoc />
/// </summary>
public class WslApi : IWslApi
{
    private readonly IIo _io;
    private readonly IEnvironment _environment;
    private readonly IRegistry _registry;

    /// <summary>
    /// <inheritdoc cref="IWslApi"/>
    /// </summary>
    public WslApi(IRegistry? registry = null, IIo? io = null, IEnvironment? environment = null)
    {
        _registry = registry ?? new Win32Registry();
        _io = io ?? new Win32IO();
        _environment = environment ?? new Win32Environment();
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool IsWslSupported()
    {
        return IsWslSupported(out _);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool IsWslSupported(out string? missingCapabilities)
    {
        missingCapabilities = null;

        var commonErrorMessage =
            "Windows Subsystems for Linux requires 64-bit system and latest version of Windows 10 or higher than Windows Server 1709.";

        if (!_environment.Is64BitOperatingSystem || !_environment.Is64BitProcess)
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        if (_environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        if (
            _environment.OSVersion.Version.Major < 10
            || _environment.OSVersion.Version.Minor < 0
            || _environment.OSVersion.Version.Build < 16299
        )
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool IsInstalled
    {
        get
        {
            var systemDirectory = _environment.GetFolderPath(Environment.SpecialFolder.System);

            if (!_io.FileExists(_io.Combine(systemDirectory, "wslapi.dll")))
            {
                return false;
            }

            if (!_io.FileExists(_io.Combine(systemDirectory, "wsl.exe")))
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IReadOnlyList<DistroInfo> GetDistributionList()
    {
        if (!this.IsWslSupported(out var missingCapabilities))
        {
            throw new PlatformNotSupportedException(missingCapabilities);
        }

        var currentUser = _registry.GetCurrentUser();
        var lxssPath = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Lxss");

        using var lxssKey = currentUser.OpenSubKey(lxssPath);
        var defaultGuid = lxssKey.GetValue<Guid>("DefaultDistribution");

        return lxssKey
            .GetSubKeyNames()
            .Select(
                (keyName) =>
                    ReadFromRegistryKey(
                        lxssKey.OpenSubKey(keyName)!,
                        Guid.Parse((string)keyName),
                        defaultGuid
                    )
            )
            .Where((d) => d.HasValue)
            .Select((d) => d!.Value)
            .ToList()
            .AsReadOnly();
    }

    private DistroInfo? ReadFromRegistryKey(
        IRegistryKey distroKey,
        Guid parsedGuid,
        Guid? parsedDefaultGuid
    )
    {
        var distroName = distroKey.GetValue<string>("DistributionName");
        var basePath = distroKey.GetValue<string>("BasePath");
        var normalizedPath = _io.GetFullPath(basePath);
        var kernelCommandLine = distroKey.GetValue("KernelCommandLine", String.Empty);

        return new DistroInfo()
        {
            DistroId = parsedGuid,
            DistroName = distroName,
            BasePath = normalizedPath,
            KernelCommandLine = kernelCommandLine.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            ),
            IsDefault = parsedDefaultGuid.HasValue && parsedDefaultGuid.Value.Equals(parsedGuid),
            WslVersion = distroKey.GetValue<int>("Version"),
            DistroFlags = (DistroFlags)distroKey.GetValue<int>("Flags"),
            DefaultUid = 0,
            DefaultEnvironmentVariables = Array.Empty<string>()
        };
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public DistroInfo? GetDefaultDistribution()
    {
        return GetDistributionList().FirstOrDefault((d) => d.IsDefault);
    }
}
