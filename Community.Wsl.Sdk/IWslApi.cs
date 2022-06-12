using System.Collections.Generic;

namespace Community.Wsl.Sdk;

/// <summary>
/// Provides functionality to help you call WSL from .NET applications.
/// </summary>
public interface IWslApi
{
    /// <summary>
    /// Checks if is WSL supported. This check is independent of <see cref="IsInstalled"/>.
    /// </summary>
    public bool IsWslSupported();

    /// <summary>
    /// Checks if WSL is installed. WSL being installed implies that <see cref="IsWslSupported()"/> to return <c>true</c>, too.
    /// </summary>
    public bool IsInstalled { get; }

    /// <summary>
    /// Checks if the environment you are running in now supports WSL.
    /// The error message is returned as out parameter. If wsl is supported, <paramref name="missingCapabilities"/> is <c>null</c>.
    /// </summary>
    public bool IsWslSupported(out string? missingCapabilities);

    /// <summary>
    /// Returns information about the default WSL distribution.
    /// </summary>
    /// <returns>
    /// Returns default WSL distribution information.
    /// Returns null if no WSL distribution is installed or no distribution is set as the default.
    /// </returns>
    public DistroInfo? GetDefaultDistribution();

    /// <summary>
    /// Returns all installed WSL distributions.
    /// </summary>
    /// <returns>Returns a list of information about the installed WSL distributions.</returns>
    public IReadOnlyList<DistroInfo> GetDistributionList();
}
