using System;
using System.Threading.Tasks;

namespace Community.Wsl.Sdk;

/// <summary>
/// Interface for a command that is used to execute programs in a WSL distribution.
/// </summary>
public interface ICommand : IDisposable
{
    /// <summary>
    /// Returns <c>true</c> when the command has been started but hasn't finished, yet. Otherwise <c>false</c>.
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    ///Returns <c>true</c> when the result of the command is being awaited. Otherwise <c>false</c> is returned.
    /// </summary>
    bool HasWaited { get; }

    /// <summary>
    /// Returns <c>true</c> when the native resources have been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Returns <c>true</c> when the command has been started and exited.
    /// </summary>
    bool HasExited { get; }

    /// <summary>
    /// Starts the command and can only be executed once.
    /// This method will block the executing thread and not wait for the command to finish.
    /// </summary>
    /// <returns>
    /// Streams of the command.
    /// </returns>
    CommandStreams Start();

    /// <summary>
    /// Waits until the started command has exited and returns the result.
    /// This method will block the executing thread.
    /// The command needs to be started first (see <see cref="Start"/>).
    /// </summary>
    /// <returns>
    /// The result of the command execution.
    /// </returns>
    CommandResult WaitAndGetResults();

    /// <summary>
    /// Waits until the started command has exited and returns the result.
    /// This method will <b>not</b> block the executing thread.
    /// The command needs to be started first (see <see cref="Start"/>).
    /// </summary>
    /// <returns>
    /// The result of the command execution.
    /// </returns>
    Task<CommandResult> WaitAndGetResultsAsync();

    /// <summary>
    /// Starts and waits until the started command has exited and returns the result.
    /// This method will block the executing thread.
    /// The command must <b>not</b> be started first (see <see cref="Start"/>).
    /// This method executes <see cref="Start"/> and <see cref="WaitAndGetResults"/> internally.
    /// </summary>
    /// <returns>
    /// The result of the command execution.
    /// </returns>
    CommandResult StartAndGetResults();

    /// <summary>
    /// Starts and waits until the started command has exited and returns the result.
    /// This method will <b>not</b> block the executing thread.
    /// The command must <b>not</b> be started first (see <see cref="Start"/>).
    /// This method executes <see cref="Start"/> and <see cref="WaitAndGetResultsAsync"/> internally.
    /// </summary>
    /// <returns>
    /// The result of the command execution.
    /// </returns>
    Task<CommandResult> StartAndGetResultsAsync();
}
