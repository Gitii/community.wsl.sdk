using System;
using System.Threading.Tasks;

namespace Community.Wsl.Sdk.Strategies.Commands;

public interface ICommand : IDisposable
{
    bool IsStarted { get; }
    bool HasWaited { get; }
    bool IsDisposed { get; }
    bool HasExited { get; }
    CommandStreams Start();
    CommandResult WaitAndGetResults();
    Task<CommandResult> WaitAndGetResultsAsync();
    CommandResult StartAndGetResults();
    Task<CommandResult> StartAndGetResultsAsync();
}
