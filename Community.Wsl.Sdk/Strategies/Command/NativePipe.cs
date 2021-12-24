using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Community.Wsl.Sdk.Strategies.Command;

public readonly struct NativePipe : IDisposable
{
    public readonly SafeFileHandle? ReadHandle { get; init; }
    public readonly SafeFileHandle? WriteHandle { get; init; }
    public readonly StreamReader Reader { get; init; }
    public readonly StreamWriter Writer { get; init; }

    public NativePipe(SafeFileHandle? readHandle = null, SafeFileHandle? writeHandle = null)
    {
        ReadHandle = readHandle;
        WriteHandle = writeHandle;

        Reader = StreamReader.Null;
        Writer = StreamWriter.Null;
    }

    public NativePipe((SafeFileHandle readHandle, SafeFileHandle writeHandle) tuple)
    {
        ReadHandle = tuple.readHandle;
        WriteHandle = tuple.writeHandle;

        Reader = StreamReader.Null;
        Writer = StreamWriter.Null;
    }

    public void Dispose()
    {
        ReadHandle?.Dispose();
        WriteHandle?.Dispose();
        Reader?.Close();
        Reader?.Dispose();
        Writer?.Close();
        Writer?.Dispose();
    }
}
