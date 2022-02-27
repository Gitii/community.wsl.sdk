using System.Diagnostics;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32ProcessManager : IProcessManager
{
    public IProcess? Start(ProcessStartInfo startInfo)
    {
        var process = Process.Start(startInfo);
        if (process == null)
        {
            return null;
        }

        return new Win32Process(process);
    }
}
