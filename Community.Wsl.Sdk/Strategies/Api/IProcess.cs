using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Community.Wsl.Sdk.Strategies.Api;

/// <summary>
/// Wrapper for a <see cref="Process"/>.
/// <inheritdoc cref="Process"/>
/// </summary>
public interface IProcess : IDisposable
{
    /// <summary>
    /// <inheritdoc cref="Process.HasExited"/>
    /// </summary>
    public bool HasExited { get; }

    /// <summary>
    /// <inheritdoc cref="Process.StandardOutput"/>
    /// </summary>
    public StreamReader StandardOutput { get; }

    /// <summary>
    /// <inheritdoc cref="Process.StandardError"/>
    /// </summary>
    public StreamReader StandardError { get; }

    /// <summary>
    /// <inheritdoc cref="Process.StandardInput"/>
    /// </summary>
    public StreamWriter StandardInput { get; }

    /// <summary>
    /// <inheritdoc cref="Process.EnableRaisingEvents"/>
    /// </summary>
    public bool EnableRaisingEvents { get; set; }

    /// <summary>
    /// <inheritdoc cref="Process.Exited"/>
    /// </summary>
    public event EventHandler Exited;

    /// <summary>
    /// <inheritdoc cref="Process.ExitCode"/>
    /// </summary>
    public int ExitCode { get; }

    /// <summary>
    /// <inheritdoc cref="Process.WaitForExit()"/>
    /// </summary>
    public void WaitForExit();
}
