﻿using System;
using Microsoft.Win32.SafeHandles;

namespace Community.Wsl.Sdk.Strategies.NativeMethods;

public partial class Win32NativeMethods : BaseNativeMethods
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
        return NativeMethods.CoInitializeSecurity(
            pSecDesc,
            cAuthSvc,
            asAuthSvc,
            pReserved1,
            dwAuthnLevel,
            dwImpLevel,
            pAuthList,
            dwCapabilities,
            pReserved3
        );
    }

    public override bool WslIsDistributionRegistered(string distributionName)
    {
        return NativeMethods.WslIsDistributionRegistered(distributionName);
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
        return NativeMethods.WslGetDistributionConfiguration(
            distributionName,
            out distributionVersion,
            out defaultUID,
            out wslDistributionFlags,
            out defaultEnvironmentVariables,
            out defaultEnvironmentVariableCount
        );
    }

    public override IntPtr GetCurrentProcess()
    {
        return NativeMethods.GetCurrentProcess();
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
        return NativeMethods.WslLaunch(
            distributionName,
            command,
            useCurrentWorkingDirectory,
            stdIn,
            stdOut,
            stdErr,
            out process
        );
    }

    public override bool CreatePipe(
        out SafeFileHandle hReadPipe,
        out SafeFileHandle hWritePipe,
        in SECURITY_ATTRIBUTES lpPipeAttributes,
        int nSize
    )
    {
        return NativeMethods.CreatePipe(out hReadPipe, out hWritePipe, in lpPipeAttributes, nSize);
    }

    public virtual bool ReadFile(
        SafeFileHandle hFile,
        IntPtr lpBuffer,
        int nNumberOfBytesToRead,
        out int lpNumberOfBytesRead,
        IntPtr lpOverlapped
    )
    {
        return NativeMethods.ReadFile(
            hFile,
            lpBuffer,
            nNumberOfBytesToRead,
            out lpNumberOfBytesRead,
            lpOverlapped
        );
    }

    public override IntPtr GetStdHandle(int nStdHandle)
    {
        return NativeMethods.GetStdHandle(nStdHandle);
    }

    public override int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds)
    {
        return NativeMethods.WaitForSingleObject(hHandle, dwMilliseconds);
    }

    public override bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode)
    {
        return NativeMethods.GetExitCodeProcess(hProcess, out lpExitCode);
    }

    public override bool CloseHandle(SafeFileHandle hObject)
    {
        return NativeMethods.CloseHandle(hObject);
    }

    public virtual bool CloseHandle(IntPtr hObject)
    {
        return NativeMethods.CloseHandle(hObject);
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
        return NativeMethods.DuplicateHandle(
            hSourceProcessHandle,
            hSourceHandle,
            hTargetProcessHandle,
            out lpTargetHandle,
            dwDesiredAccess,
            bInheritHandle,
            dwOptions
        );
    }
}
