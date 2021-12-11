using System;
using System.IO;
using System.Threading;

namespace Wslhub.Sdk.Strategies.Command;

internal class StreamDataReader : IStreamReader
{
    private StreamReader _reader;
    private Thread? _thread;
    private byte[]? _data;

    public StreamDataReader(StreamReader reader)
    {
        _reader = reader;
    }

    public byte[]? Data => _data;

    private void Finished(byte[] data)
    {
        _data = data;
    }

    public void Fetch()
    {
        if (_thread != null)
        {
            throw new ArgumentException("Already started fetching!");
        }

        _thread = new Thread(
            () =>
            {
                StreamReader reader = _reader;
                MemoryStream stream = new MemoryStream();

                reader.BaseStream.CopyTo(stream);

                Finished(stream.ToArray());
            }
        );

        _thread.Start();
    }

    public void CopyResultTo(ref CommandResult result, bool isStdOut)
    {
        if (_thread == null)
        {
            throw new ArgumentException("Data hasn't been fetched, yet!");
        }

        if (_thread.ThreadState != ThreadState.Stopped)
        {
            throw new ArgumentException("Fetching hasn't been finished, yet!");
        }

        if (isStdOut)
        {
            result = result with { StdoutData = Data, Stdout = null };
        }
        else
        {
            result = result with { StderrData = Data, Stderr = null };
        }
    }
}
