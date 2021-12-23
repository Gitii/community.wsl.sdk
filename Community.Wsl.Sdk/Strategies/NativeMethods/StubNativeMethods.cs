using System;
using Microsoft.Win32.SafeHandles;

namespace Community.Wsl.Sdk.Strategies.NativeMethods;

public class StubNativeMethods : BaseNativeMethods
{
    public override int CoInitializeSecurity(
        IntPtr pSecDesc,
        int cAuthSvc,
        IntPtr asAuthSvc,
        IntPtr pReserved1,
        RpcAuthnLevel dwAuthnLevel,
        RpcImpLevel dwImpLevel,
        IntPtr pAuthList,
        EoAuthnCap dwCapabilities,
        IntPtr pReserved3
    )
    {
        return 0;
    }

    public override bool WslIsDistributionRegistered(string distributionName)
    {
        return true;
    }

    public override int WslGetDistributionConfiguration(
        string distributionName,
        out int distributionVersion,
        out int defaultUID,
        out DistroFlags wslDistributionFlags,
        out IntPtr defaultEnvironmentVariables,
        out int defaultEnvironmentVariableCount
    )
    {
        distributionVersion = 2;
        defaultUID = 0;
        wslDistributionFlags = DistroFlags.AppendNtPath;
        defaultEnvironmentVariables = IntPtr.Zero;
        defaultEnvironmentVariableCount = 0;

        return 0;
    }

    public override IntPtr GetCurrentProcess()
    {
        return IntPtr.Zero;
    }

    public override int WslLaunch(
        string distributionName,
        string command,
        bool useCurrentWorkingDirectory,
        SafeFileHandle stdIn,
        SafeFileHandle stdOut,
        SafeFileHandle stdErr,
        out IntPtr process
    )
    {
        process = IntPtr.Zero;
        return 0;
    }

    public override bool CreatePipe(
        out SafeFileHandle hReadPipe,
        out SafeFileHandle hWritePipe,
        in SECURITY_ATTRIBUTES lpPipeAttributes,
        int nSize
    )
    {
        hReadPipe = new SafeFileHandle(IntPtr.Zero, false);
        hWritePipe = new SafeFileHandle(IntPtr.Zero, false);

        return true;
    }

    public override IntPtr GetStdHandle(int nStdHandle)
    {
        return IntPtr.Zero;
    }

    public override int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds)
    {
        return 0;
    }

    public override bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode)
    {
        lpExitCode = 0;
        return true;
    }

    public override bool CloseHandle(SafeFileHandle hObject)
    {
        return true;
    }

    public override bool DuplicateHandle(
        SafeFileHandle hSourceProcessHandle,
        SafeFileHandle hSourceHandle,
        SafeFileHandle hTargetProcessHandle,
        out SafeFileHandle lpTargetHandle,
        uint dwDesiredAccess,
        bool bInheritHandle,
        DuplicateOptions dwOptions
    )
    {
        lpTargetHandle = new SafeFileHandle(IntPtr.Zero, false);
        return true;
    }
}
