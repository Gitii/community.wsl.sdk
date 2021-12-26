using System;
using System.Diagnostics;
using System.IO;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32Process : IProcess
{
    private readonly Process _process;

    public Win32Process(Process process)
    {
        _process = process;
    }

    public void Dispose()
    {
        _process.Dispose();
    }

    public bool HasExited => _process.HasExited;

    public StreamReader StandardOutput => _process.StandardOutput;

    public StreamReader StandardError => _process.StandardError;

    public StreamWriter StandardInput => _process.StandardInput;

    public bool EnableRaisingEvents
    {
        get => _process.EnableRaisingEvents;
        set => _process.EnableRaisingEvents = value;
    }

    public event EventHandler? Exited
    {
        add => _process.Exited += value;
        remove => _process.Exited -= value;
    }

    public int ExitCode => _process.ExitCode;

    public void WaitForExit()
    {
        _process.WaitForExit();
    }
}
