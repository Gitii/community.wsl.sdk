using System;
using Community.Wsl.Sdk.Strategies.Api;

namespace Community.Wsl.Sdk;

/// <summary>
/// A POD class that contains information about a wsl distribution.
/// </summary>
public readonly struct DistroInfo
{
    /// <summary>
    /// Unique ID identifying the WSL distribution
    /// </summary>
    public Guid DistroId { get; init; }

    /// <summary>
    /// Name of the WSL distribution
    /// </summary>
    public string DistroName { get; init; }

    /// <summary>
    /// List of kernel parameters to be passed on cold boot
    /// </summary>
    public string[] KernelCommandLine { get; init; }

    /// <summary>
    /// The path to the local directory where the WSL distribution is installed.
    /// </summary>
    public string BasePath { get; init; }

    /// <summary>
    /// Whether or not registered as the default WSL distribution
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Default environment variables set in the distribution.
    /// </summary>
    public string[] DefaultEnvironmentVariables { get; init; }

    /// <summary>
    /// The UID of the user to use when running the distribution.
    /// </summary>
    public int DefaultUid { get; init; }

    /// <summary>
    /// Represents the default settings of the distribution.
    /// </summary>
    public DistroFlags DistroFlags { get; init; }

    /// <summary>
    /// Determine which version of the WSL runtime is configured to use.
    /// </summary>
    public int WslVersion { get; init; }

    /// <summary>
    /// Returns a description of this model object.
    /// </summary>
    /// <returns>Returns a description of this model object.</returns>
    public override string ToString() => $"{DistroName} [{DistroId}]";
}
