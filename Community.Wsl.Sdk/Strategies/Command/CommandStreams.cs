using System.IO;

namespace Community.Wsl.Sdk.Strategies.Command;

/// <summary>
/// Encapsulates the io stream of a <see cref="ICommand"/>.
/// </summary>
public class CommandStreams
{
    /// <summary>
    /// The standard input stream.
    /// Will be <see cref="StreamWriter.Null"/> if <see cref="CommandExecutionOptions.StdInDataProcessingMode"/> equals <see cref="DataProcessingMode.Drop"/>.
    /// </summary>
    public StreamWriter StandardInput { get; init; } = null!;

    /// <summary>
    /// The standard output stream.
    /// Will be <see cref="StreamReader.Null"/> if <see cref="CommandExecutionOptions.StdoutDataProcessingMode"/> equals <see cref="DataProcessingMode.Drop"/>.
    /// </summary>
    public StreamReader StandardOutput { get; init; } = null!;

    /// <summary>
    /// The standard error stream.
    /// Will be <see cref="StreamReader.Null"/> if <see cref="CommandExecutionOptions.StdErrDataProcessingMode"/> equals <see cref="DataProcessingMode.Drop"/>.
    /// </summary>
    public StreamReader StandardError { get; init; } = null!;
}
