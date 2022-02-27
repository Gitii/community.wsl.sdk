using System;

namespace Community.Wsl.Sdk.Strategies.Api;

/// <summary>
/// The enumeration specifies the behavior of a distribution in the Windows Subsystem for Linux (WSL).
/// </summary>
[Flags]
public enum DistroFlags
{
    /// <summary>
    /// No flags are being supplied.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// Allow the distribution to interoperate with Windows processes (for example, the user can invoke "cmd.exe" or "notepad.exe" from within a WSL session).
    /// </summary>
    EnableInterop = 0x1,

    /// <summary>
    /// Add the Windows %PATH% environment variable values to WSL sessions.
    /// </summary>
    AppendNtPath = 0x2,

    /// <summary>
    /// Automatically mount Windows drives inside of WSL sessions (for example, "C:" will be available under "/mnt/c").
    /// </summary>
    EnableDriveMouting = 0x4
}