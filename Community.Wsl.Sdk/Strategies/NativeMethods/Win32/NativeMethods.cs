using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Community.Wsl.Sdk.Strategies.NativeMethods.Win32;

internal static class NativeMethods
{
    [SecurityCritical]
    [DllImport("ole32.dll", ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern int CoInitializeSecurity(
        IntPtr pSecDesc,
        int cAuthSvc,
        IntPtr asAuthSvc,
        IntPtr pReserved1,
        [MarshalAs(UnmanagedType.U4)] BaseNativeMethods.RpcAuthnLevel dwAuthnLevel,
        [MarshalAs(UnmanagedType.U4)] BaseNativeMethods.RpcImpLevel dwImpLevel,
        IntPtr pAuthList,
        [MarshalAs(UnmanagedType.U4)] BaseNativeMethods.EoAuthnCap dwCapabilities,
        IntPtr pReserved3
    );

    [SecurityCritical]
    [DllImport(
        "wslapi.dll",
        CallingConvention = CallingConvention.Winapi,
        CharSet = CharSet.Unicode,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WslIsDistributionRegistered(string distributionName);

    [SecurityCritical]
    [DllImport(
        "wslapi.dll",
        CallingConvention = CallingConvention.Winapi,
        CharSet = CharSet.Unicode,
        ExactSpelling = true,
        PreserveSig = true
    )]
    public static extern int WslGetDistributionConfiguration(
        string distributionName,
        [MarshalAs(UnmanagedType.I4)] out int distributionVersion,
        [MarshalAs(UnmanagedType.I4)] out int defaultUID,
        [MarshalAs(UnmanagedType.I4)] out BaseNativeMethods.DistroFlags wslDistributionFlags,
        out IntPtr defaultEnvironmentVariables,
        [MarshalAs(UnmanagedType.I4)] out int defaultEnvironmentVariableCount
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        ExactSpelling = true,
        SetLastError = true
    )]
    public static extern IntPtr GetCurrentProcess();

    [SecurityCritical]
    [DllImport(
        "wslapi.dll",
        CallingConvention = CallingConvention.Winapi,
        CharSet = CharSet.Unicode,
        ExactSpelling = true,
        PreserveSig = true
    )]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern int WslLaunch(
        string distributionName,
        string command,
        bool useCurrentWorkingDirectory,
        SafeFileHandle stdIn,
        SafeFileHandle stdOut,
        SafeFileHandle stdErr,
        out IntPtr process
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreatePipe(
        out SafeFileHandle hReadPipe,
        out SafeFileHandle hWritePipe,
        in BaseNativeMethods.SECURITY_ATTRIBUTES lpPipeAttributes,
        [MarshalAs(UnmanagedType.U4)] int nSize
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadFile(
        SafeFileHandle hFile,
        IntPtr lpBuffer,
        [MarshalAs(UnmanagedType.U4)] int nNumberOfBytesToRead,
        [MarshalAs(UnmanagedType.U4)] out int lpNumberOfBytesRead,
        IntPtr lpOverlapped
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    public static extern IntPtr GetStdHandle([MarshalAs(UnmanagedType.U4)] int nStdHandle);

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern int WaitForSingleObject(
        IntPtr hHandle,
        [MarshalAs(UnmanagedType.U4)] int dwMilliseconds
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetExitCodeProcess(
        IntPtr hProcess,
        [MarshalAs(UnmanagedType.U4)] out int lpExitCode
    );

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(SafeFileHandle hObject);

    [SecurityCritical]
    [DllImport(
        "kernel32.dll",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
        SafeFileHandle hSourceProcessHandle,
        SafeFileHandle hSourceHandle,
        SafeFileHandle hTargetProcessHandle,
        out SafeFileHandle lpTargetHandle,
        uint dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        BaseNativeMethods.DuplicateOptions dwOptions
    );
}
