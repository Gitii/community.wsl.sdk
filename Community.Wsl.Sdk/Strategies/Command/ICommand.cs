using System;

namespace Wslhub.Sdk.Strategies.Command;

public interface ICommand : IDisposable
{
    bool IsStarted { get; }
    bool HasWaited { get; }
    bool IsDisposed { get; }
    bool HasExited { get; }
    CommandStreams Start();
    CommandResult WaitAndGetResults();
}