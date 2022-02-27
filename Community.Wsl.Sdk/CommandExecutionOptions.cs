using System.Text;

namespace Community.Wsl.Sdk;

public struct CommandExecutionOptions
{
    public CommandExecutionOptions()
    {
        FailOnNegativeExitCode = true;

        StdoutDataProcessingMode = DataProcessingMode.Drop;
        StdErrDataProcessingMode = DataProcessingMode.Drop;
        StdInDataProcessingMode = DataProcessingMode.Drop;

        StdoutEncoding = null;
        StderrEncoding = null;
        StdinEncoding = null;
    }

    public bool FailOnNegativeExitCode { get; set; } = true;

    public DataProcessingMode StdoutDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public DataProcessingMode StdErrDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public DataProcessingMode StdInDataProcessingMode { get; set; } = DataProcessingMode.Drop;

    public Encoding? StdoutEncoding { get; set; }

    public Encoding? StderrEncoding { get; set; }

    public Encoding? StdinEncoding { get; set; }
}
