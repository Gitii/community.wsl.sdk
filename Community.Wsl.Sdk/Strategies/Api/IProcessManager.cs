using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Community.Wsl.Sdk.Strategies.Api;

/// <summary>
/// Wrapper for <see cref="Process.Start(System.Diagnostics.ProcessStartInfo)"/>.
/// Returns an <see cref="Process"/> wrapped in a <see cref="IProcess"/>.
/// </summary>
public interface IProcessManager
{
    /// <summary>
    /// <inheritdoc cref="Process.Start(System.Diagnostics.ProcessStartInfo)"/>
    /// </summary>
    /// <param name="startInfo"><inheritdoc cref="Process.Start(System.Diagnostics.ProcessStartInfo)" path="/param[@name='startInfo']"/></param>
    /// <returns></returns>
    public abstract IProcess? Start(ProcessStartInfo startInfo);
}
