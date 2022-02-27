﻿using System.Threading.Tasks;

namespace Community.Wsl.Sdk.Strategies.Command;

internal class StreamNullReader : IStreamReader
{
    public void Fetch() { }

    public void CopyResultTo(ref CommandResult result, bool isStdOut) { }

    public void Wait() { }

    public Task WaitAsync()
    {
        return Task.CompletedTask;
    }
}
