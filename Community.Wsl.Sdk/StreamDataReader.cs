﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Community.Wsl.Sdk;

internal class StreamDataReader : IStreamReader
{
    protected readonly StreamReader _reader;
    private Thread? _thread;
    private byte[]? _data;
    private TaskCompletionSource<byte[]>? _completionSource;

    public StreamDataReader(StreamReader reader)
    {
        _reader = reader;
    }

    public byte[]? Data => _data;

    protected virtual void Finished(byte[] data)
    {
        _data = data;
        _completionSource?.SetResult(data);
    }

    public void Fetch()
    {
        if (_thread != null)
        {
            throw new Exception("Already started fetching!");
        }

        _completionSource = new TaskCompletionSource<byte[]>();

        _thread = new Thread(
            () =>
            {
                StreamReader reader = _reader;
                MemoryStream stream = new MemoryStream();

                reader.BaseStream.CopyTo(stream);

                var data = stream.ToArray();
                Finished(data);
            }
        ) { IsBackground = true };

        _thread.Start();
    }

    public virtual void CopyResultTo(ref CommandResult result, bool isStdOut)
    {
        if (_thread == null)
        {
            throw new Exception("Data hasn't been fetched, yet!");
        }

        if (!_completionSource?.Task.IsCompleted ?? false)
        {
            throw new Exception("Fetching hasn't been finished, yet!");
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

    public void Wait()
    {
        _thread?.Join();
    }

    public Task WaitAsync()
    {
        if (_completionSource == null)
        {
            return Task.CompletedTask;
        }

        return _completionSource.Task;
    }
}
