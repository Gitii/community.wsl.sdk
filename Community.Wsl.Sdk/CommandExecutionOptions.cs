using System.Text;

namespace Community.Wsl.Sdk;

public struct CommandExecutionOptions
{
    public bool FailOnNegativeExitCode { get; set; } = true;

    public DataProcessingMode StdoutDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public DataProcessingMode StdErrDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public DataProcessingMode StdInDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public Encoding? StdoutEncoding { get; set; }

    public Encoding? StderrEncoding { get; set; }

    public Encoding? StdinEncoding { get; set; }
}
