using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Wslhub.Sdk.Strategies.NativeMethods
{
    public abstract class BaseNativeMethods
    {
        public abstract int CoInitializeSecurity(
            IntPtr pSecDesc,
            int cAuthSvc,
            IntPtr asAuthSvc,
            IntPtr pReserved1,
            RpcAuthnLevel dwAuthnLevel,
            RpcImpLevel dwImpLevel,
            IntPtr pAuthList,
            EoAuthnCap dwCapabilities,
            IntPtr pReserved3
        );

        public enum RpcAuthnLevel
        {
            Default = 0,
            None = 1,
            Connect = 2,
            Call = 3,
            Pkt = 4,
            PktIntegrity = 5,
            PktPrivacy = 6
        }

        public enum RpcImpLevel
        {
            Default = 0,
            Anonymous = 1,
            Identify = 2,
            Impersonate = 3,
            Delegate = 4
        }

        public enum EoAuthnCap
        {
            None = 0x00,
            MutualAuth = 0x01,
            StaticCloaking = 0x20,
            DynamicCloaking = 0x40,
            AnyAuthority = 0x80,
            MakeFullSIC = 0x100,
            Default = 0x800,
            SecureRefs = 0x02,
            AccessControl = 0x04,
            AppID = 0x08,
            Dynamic = 0x10,
            RequireFullSIC = 0x200,
            AutoImpersonate = 0x400,
            NoCustomMarshal = 0x2000,
            DisableAAA = 0x1000
        }

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

        public abstract bool WslIsDistributionRegistered(string distributionName);

        public abstract int WslGetDistributionConfiguration(
            string distributionName,
            out int distributionVersion,
            out int defaultUID,
            out DistroFlags wslDistributionFlags,
            out IntPtr defaultEnvironmentVariables,
            out int defaultEnvironmentVariableCount
        );

        public abstract IntPtr GetCurrentProcess();

        public abstract int WslLaunch(
            string distributionName,
            string command,
            bool useCurrentWorkingDirectory,
            SafeFileHandle stdIn,
            SafeFileHandle stdOut,
            SafeFileHandle stdErr,
            out IntPtr process
        );

        public abstract bool CreatePipe(
            out SafeFileHandle hReadPipe,
            out SafeFileHandle hWritePipe,
            in SECURITY_ATTRIBUTES lpPipeAttributes,
            int nSize
        );

        public abstract bool ReadFile(
            SafeFileHandle hFile,
            IntPtr lpBuffer,
            int nNumberOfBytesToRead,
            out int lpNumberOfBytesRead,
            IntPtr lpOverlapped
        );

        public const int E_INVALIDARG = unchecked((int)0x80070057);

        public const int STD_INPUT_HANDLE = -10;

        public const int STD_OUTPUT_HANDLE = -11;

        public const int STD_ERROR_HANDLE = -12;

        public abstract IntPtr GetStdHandle(int nStdHandle);

        public const int INFINITE = unchecked((int)0xFFFFFFFF);

        public abstract int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        public const int WAIT_ABANDONED = 0x00000080,
            WAIT_OBJECT_0 = 0x00000000,
            WAIT_TIMEOUT = 0x00000102,
            WAIT_FAILED = unchecked((int)0xFFFFFFFF);

        public abstract bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);

        public abstract bool CloseHandle(SafeFileHandle hObject);

        public abstract bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OSVERSIONINFOEXW
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;

            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            [MarshalAs(UnmanagedType.U4)]
            public int nLength;

            public IntPtr lpSecurityDescriptor;

            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        public abstract bool DuplicateHandle(
            SafeFileHandle hSourceProcessHandle,
            SafeFileHandle hSourceHandle,
            SafeFileHandle hTargetProcessHandle,
            out SafeFileHandle lpTargetHandle,
            uint dwDesiredAccess,
            bool bInheritHandle,
            DuplicateOptions dwOptions
        );

        [Flags]
        public enum DuplicateOptions : uint
        {
            DUPLICATE_CLOSE_SOURCE = (0x00000001), // Closes the source handle. This occurs regardless of any error status returned.
            DUPLICATE_SAME_ACCESS = (0x00000002) //Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
        }

        public const uint STILL_ACTIVE = 0x00000103;
    }
}
