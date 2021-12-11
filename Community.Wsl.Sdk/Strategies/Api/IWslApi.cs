using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wslhub.Sdk.Strategies.Api;

/// <summary>
/// Provides functionality to help you call WSL from .NET applications.
/// </summary>
public interface IWslApi
{
    /// <summary>
    /// Checks if the environment you are running in now supports WSL.
    /// </summary>
    public bool IsWslSupported()
    {
        return IsWslSupported(out _);
    }

    /// <summary>
    /// Checks if the environment you are running in now supports WSL.
    /// The error message is returned as out parameter. If wsl is supported, <paramref name="missingCapabilities"/> is <c>null</c>.
    /// </summary>
    public bool IsWslSupported(out string? missingCapabilities)
    {
        missingCapabilities = null;

        var commonErrorMessage =
            "Windows Subsystems for Linux requires 64-bit system and latest version of Windows 10 or higher than Windows Server 1709.";

        if (!Environment.Is64BitOperatingSystem || !Environment.Is64BitProcess)
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        if (
            Environment.OSVersion.Version.Major < 10
            || Environment.OSVersion.Version.Minor < 0
            || Environment.OSVersion.Version.Build < 16299
        )
        {
            missingCapabilities = commonErrorMessage;
            return false;
        }

        var systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);

        if (!File.Exists(Path.Combine(systemDirectory, "wslapi.dll")))
        {
            missingCapabilities = "This system does not have WSL enabled.";
            return false;
        }

        if (!File.Exists(Path.Combine(systemDirectory, "wsl.exe")))
        {
            missingCapabilities = "This system does not have wsl.exe CLI.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns information about the default WSL distribution.
    /// </summary>
    /// <returns>
    /// Returns default WSL distribution information.
    /// Returns null if no WSL distro is installed or no distro is set as the default.
    /// </returns>
    public DistroInfo? GetDefaultDistro()
    {
        return GetDistroList().FirstOrDefault((d) => d.IsDefault);
    }

    /// <summary>
    /// Returns all installed WSL distributions.
    /// </summary>
    /// <returns>Returns a list of information about the installed WSL distributions.</returns>
    public IReadOnlyList<DistroInfo> GetDistroList();
}
