using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Community.Wsl.Sdk.Tests;

[ExcludeFromCodeCoverage]
public class BlockingReadOnlyStream : Stream
{
    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        Thread.Sleep(5000);
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new Exception();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead { get; } = true;
    public override bool CanSeek { get; } = false;
    public override bool CanWrite { get; } = false;

    public override long Length => throw new Exception();

    public override long Position
    {
        get { throw new Exception(); }
        set { throw new Exception(); }
    }
}
