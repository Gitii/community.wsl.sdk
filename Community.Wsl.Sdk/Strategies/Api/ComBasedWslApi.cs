using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Community.Wsl.Sdk.Strategies.NativeMethods;

namespace Community.Wsl.Sdk.Strategies.Api;

/// <summary>
/// <inheritdoc cref="IWslApi"/>
/// </summary>
public class ComBasedWslApi : IWslApi
{
    private readonly BaseNativeMethods _nativeMethods;
    private readonly IRegistry _registry;

    public ComBasedWslApi(BaseNativeMethods? nativeMethods = null, IRegistry? registry = null)
    {
        _nativeMethods = nativeMethods ?? new Win32NativeMethods();
        _registry = registry ?? new Win32Registry();
    }

    /// <summary>
    /// <inheritdoc cref="IWslApi.InitializeSecurityModel"/>
    /// </summary>
    public void InitializeSecurityModel()
    {
        var result = _nativeMethods.CoInitializeSecurity(
            IntPtr.Zero,
            (-1),
            IntPtr.Zero,
            IntPtr.Zero,
            BaseNativeMethods.RpcAuthnLevel.Default,
            BaseNativeMethods.RpcImpLevel.Impersonate,
            IntPtr.Zero,
            BaseNativeMethods.EoAuthnCap.StaticCloaking,
            IntPtr.Zero
        );

        if (result != 0)
        {
            throw new COMException("Cannot complete CoInitializeSecurity.", result);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWslApi.GetDistroList"/>
    /// </summary>
    public IReadOnlyList<DistroInfo> GetDistroList()
    {
        if (!((IWslApi)this).IsWslSupported(out var missingCapabilities))
        {
            throw new PlatformNotSupportedException(missingCapabilities);
        }

        var currentUser = _registry.GetCurrentUser();
        var lxssPath = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Lxss");

        using var lxssKey = currentUser.OpenSubKey(lxssPath);
        var defaultGuid = lxssKey.GetValue<Guid>("DefaultDistribution");

        return lxssKey
            .GetSubKeyNames()
            .Select(
                (keyName) =>
                    ReadFromRegistryKey(
                        lxssKey.OpenSubKey(keyName)!,
                        Guid.Parse(keyName),
                        defaultGuid
                    )
            )
            .Where((d) => d.HasValue)
            .Select((d) => d!.Value)
            .ToList()
            .AsReadOnly();
    }

    private unsafe string[] ReadArray(ref IntPtr arrayPtr, int count)
    {
        if (arrayPtr == IntPtr.Zero)
        {
            return Array.Empty<string>();
        }

        try
        {
            string[] array = new string[count];
            var lpEnvironmentVariables = (byte***)arrayPtr.ToPointer();

            for (int i = 0; i < count; i++)
            {
                byte** lpArray = lpEnvironmentVariables[i];
                array[i] =
                    Marshal.PtrToStringAnsi(new IntPtr(lpArray))
                    ?? throw new Exception(
                        "Failed to marshal native ansi string to managed string"
                    );
                Marshal.FreeCoTaskMem(new IntPtr(lpArray));
            }

            return array;
        }
        finally
        {
            Marshal.FreeCoTaskMem(arrayPtr);
            arrayPtr = IntPtr.Zero;
        }
    }

    private DistroInfo? ReadFromRegistryKey(
        IRegistryKey distroKey,
        Guid parsedGuid,
        Guid? parsedDefaultGuid
    )
    {
        var distroName = distroKey.GetValue("DistributionName", string.Empty);

        if (string.IsNullOrWhiteSpace(distroName))
        {
            throw new Exception("Cannot parse distribution name.");
        }

        IntPtr environmentVariables = IntPtr.Zero;

        try
        {
            var hr = _nativeMethods.WslGetDistributionConfiguration(
                distroName,
                out int distroVersion,
                out int defaultUserId,
                out BaseNativeMethods.DistroFlags flags,
                out environmentVariables,
                out int environmentVariableCount
            );

            if (hr != 0)
            {
                throw new Exception("Cannot query wsl distro configuration.");
            }

            var basePath = distroKey.GetValue<string?>("BasePath", null);
            var normalizedPath = Path.GetFullPath(
                basePath ?? throw new ArgumentException("Base path is invalid")
            );

            var kernelCommandLine = (
                distroKey.GetValue("KernelCommandLine", default(string)) as string ?? string.Empty
            );
            return new DistroInfo()
            {
                DistroId = parsedGuid,
                DistroName = distroName,
                BasePath = normalizedPath,
                KernelCommandLine = kernelCommandLine.Split(
                    new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries
                ),
                IsDefault =
                    parsedDefaultGuid.HasValue && parsedDefaultGuid.Value.Equals(parsedGuid),
                WslVersion = distroVersion,
                DistroFlags = flags,
                DefaultUid = defaultUserId,
                DefaultEnvironmentVariables = ReadArray(
                    ref environmentVariables,
                    environmentVariableCount
                )
            };
        }
        finally
        {
            Marshal.FreeCoTaskMem(environmentVariables);
        }
    }
}
