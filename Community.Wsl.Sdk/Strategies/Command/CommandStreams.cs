using System.IO;

namespace Community.Wsl.Sdk.Strategies.Command;

public class CommandStreams
{
    public StreamWriter? StandardInput { get; init; }
    public StreamReader? StandardOutput { get; init; }
    public StreamReader? StandardError { get; init; }
}
