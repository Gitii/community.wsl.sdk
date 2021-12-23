using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Community.Wsl.Sdk.Strategies.Command;

public class ManagedCommand : ICommand
{
    private ProcessStartInfo? _startInfo;
    private Process? _process;
    private bool _isStarted = false;
    private bool _hasWaited = false;
    private bool _isDisposed = false;
    private readonly string _command;
    private readonly string[] _arguments;
    private readonly CommandExecutionOptions _options;
    private readonly bool _asRoot;
    private readonly bool _shellExecute;
    private readonly string _distroName;

    private IStreamReader _stdoutReader;
    private IStreamReader _stderrReader;

    public ManagedCommand(
        string distroName,
        string command,
        string[] arguments,
        CommandExecutionOptions options,
        bool asRoot = false,
        bool shellExecute = false
    )
    {
        _options = options;
        _asRoot = asRoot;
        _shellExecute = shellExecute;
        _command = command;
        _arguments = arguments;
        _distroName = distroName;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _process?.Dispose();
            _process = null;
            _isDisposed = true;
        }
    }

    public bool IsStarted => _isStarted;

    public bool HasWaited => _hasWaited;

    public bool IsDisposed => _isDisposed;

    public bool HasExited => _process?.HasExited ?? false;

    public CommandStreams Start()
    {
        if (IsStarted)
        {
            throw new ArgumentException("Command has already been started!");
        }

        _isStarted = true;

        var wslPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            "wsl.exe"
        );

        _startInfo = new ProcessStartInfo(wslPath);

        _startInfo.ArgumentList.Add("-d");
        _startInfo.ArgumentList.Add(_distroName);

        if (_asRoot)
        {
            _startInfo.ArgumentList.Add("--system");
        }

        if (_shellExecute)
        {
            _startInfo.ArgumentList.Add("--");
        }
        else
        {
            _startInfo.ArgumentList.Add("--exec");
        }

        _startInfo.ArgumentList.Add(_command);
        foreach (string argument in _arguments)
        {
            _startInfo.ArgumentList.Add(argument);
        }

        bool redirectStandardInput = _options.StdInDataProcessingMode != DataProcessingMode.Drop;
        bool redirectStandardOutput = _options.StdoutDataProcessingMode != DataProcessingMode.Drop;
        bool redirectStandardError = _options.StdErrDataProcessingMode != DataProcessingMode.Drop;

        _startInfo.CreateNoWindow = true;
        _startInfo.RedirectStandardInput = redirectStandardInput;
        _startInfo.RedirectStandardOutput = redirectStandardOutput;
        _startInfo.RedirectStandardError = redirectStandardError;

        if (redirectStandardOutput)
        {
            _startInfo.StandardOutputEncoding = _options.StdoutEncoding ?? Console.OutputEncoding;
        }

        if (redirectStandardError)
        {
            _startInfo.StandardErrorEncoding = _options.StderrEncoding ?? Console.OutputEncoding;
        }

        if (redirectStandardInput)
        {
            _startInfo.StandardInputEncoding = _options.StdinEncoding ?? Console.InputEncoding;
        }

        var process = Process.Start(_startInfo) ?? throw new Exception("Cannot start wsl process.");

        _process = process;

        CreateReader(
            () => _process.StandardOutput,
            ref _stdoutReader,
            _options.StdoutDataProcessingMode
        );
        CreateReader(
            () => _process.StandardError,
            ref _stderrReader,
            _options.StdErrDataProcessingMode
        );

        _stdoutReader.Fetch();
        _stderrReader.Fetch();

        return new CommandStreams()
        {
            StandardInput = redirectStandardInput ? process.StandardInput : StreamWriter.Null,
            StandardOutput = redirectStandardOutput ? process.StandardOutput : StreamReader.Null,
            StandardError = redirectStandardError ? process.StandardError : StreamReader.Null
        };
    }

    private void CreateReader(
        Func<StreamReader> lazyStreamReader,
        ref IStreamReader stdoutReader,
        DataProcessingMode dataProcessingMode
    )
    {
        switch (dataProcessingMode)
        {
            case DataProcessingMode.Drop:
                stdoutReader = new StreamNullReader();
                break;
            case DataProcessingMode.Binary:
                stdoutReader = new StreamDataReader(lazyStreamReader());
                break;
            case DataProcessingMode.String:
                stdoutReader = new StreamStringReader(lazyStreamReader());
                break;
            case DataProcessingMode.External:
                stdoutReader = new StreamNullReader();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(dataProcessingMode),
                    dataProcessingMode,
                    null
                );
        }
    }

    public CommandResult WaitAndGetResults()
    {
        if (!IsStarted)
        {
            throw new ArgumentException("Command hasn't been started, yet!");
        }

        if (HasWaited)
        {
            throw new Exception("Already waited for the command to finish!");
        }

        _hasWaited = true;

        _process!.WaitForExit();

        if (_options.FailOnNegativeExitCode && _process.ExitCode != 0)
        {
            throw new Exception($"Process exit code is non-zero: {_process.ExitCode}");
        }

        var result = new CommandResult();

        _stdoutReader.CopyResultTo(ref result, true);
        _stderrReader.CopyResultTo(ref result, false);

        return result;
    }

    public async Task<CommandResult> WaitAndGetResultsAsync()
    {
        if (!IsStarted)
        {
            throw new ArgumentException("Command hasn't been started, yet!");
        }

        if (HasWaited)
        {
            throw new Exception("Already waited for the command to finish!");
        }

        _hasWaited = true;

        await WaitForExit(_process);

        if (_options.FailOnNegativeExitCode && _process.ExitCode != 0)
        {
            throw new Exception($"Process exit code is non-zero: {_process.ExitCode}");
        }

        var result = new CommandResult();

        _stdoutReader.CopyResultTo(ref result, true);
        _stderrReader.CopyResultTo(ref result, false);

        return result;
    }

    public CommandResult StartAndGetResults()
    {
        Start();
        return WaitAndGetResults();
    }

    public Task<CommandResult> StartAndGetResultsAsync()
    {
        Start();
        return WaitAndGetResultsAsync();
    }

    private Task<int> WaitForExit(Process process)
    {
        if (process.HasExited)
        {
            return Task.FromResult(process.ExitCode);
        }

        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
        process.Exited += HasExited;

        if (process.HasExited)
        {
            process.Exited -= HasExited;
            return Task.FromResult(process.ExitCode);
        }

        return tcs.Task;

        void HasExited(object _, EventArgs __)
        {
            tcs.SetResult(process.ExitCode);

            process.Exited -= HasExited;
        }
    }
}
