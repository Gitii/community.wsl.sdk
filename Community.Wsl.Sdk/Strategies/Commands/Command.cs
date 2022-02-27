using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.Api;

namespace Community.Wsl.Sdk.Strategies.Commands;

public class Command : ICommand
{
    private readonly string[] _arguments;
    private readonly bool _asRoot;
    private readonly string _command;
    private readonly string _distroName;
    private readonly CommandExecutionOptions _options;
    private readonly bool _shellExecute;

    private IEnvironment _environment;
    private bool _hasWaited = false;
    private IIo _io;
    private bool _isDisposed = false;
    private bool _isStarted = false;
    private IProcess? _process;
    private IProcessManager _processManager;
    private ProcessStartInfo? _startInfo;
    private IStreamReader _stderrReader;
    private IStreamReader _stdoutReader;

    public Command(
        string distroName,
        string command,
        string[] arguments,
        CommandExecutionOptions options,
        bool asRoot = false,
        bool shellExecute = false,
        IEnvironment? environment = null,
        IIo? io = null,
        IProcessManager? processManager = null
    )
    {
        _environment = environment ?? new Win32Environment();
        _io = io ?? new Win32IO();
        _processManager = processManager ?? new Win32ProcessManager();

        _options = options;
        _asRoot = asRoot;
        _shellExecute = shellExecute;
        _command = command;
        _arguments = arguments;
        _distroName = distroName;

        _stderrReader = new StreamNullReader();
        _stdoutReader = new StreamNullReader();

        if (options.StdInDataProcessingMode is DataProcessingMode.Binary or DataProcessingMode.String)
        {
            throw new ArgumentException("StandardInput can only be dropped or external.", nameof(options));
        }
    }

    internal IStreamReader StderrReader => _stderrReader;

    internal IStreamReader StdoutReader => _stdoutReader;

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

        bool redirectStandardInput = _options.StdInDataProcessingMode != DataProcessingMode.Drop;
        bool redirectStandardOutput = _options.StdoutDataProcessingMode != DataProcessingMode.Drop;
        bool redirectStandardError = _options.StdErrDataProcessingMode != DataProcessingMode.Drop;

        _isStarted = true;

        _startInfo = CreateStartInfo(
            redirectStandardInput,
            redirectStandardOutput,
            redirectStandardError
        );

        var process =
            _processManager.Start(_startInfo) ?? throw new Exception("Cannot start wsl process.");

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

        var result = new CommandResult() { ExitCode = _process.ExitCode };

        _stdoutReader.Wait();
        _stdoutReader.CopyResultTo(ref result, true);

        _stderrReader.Wait();
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

        var exitCode = await WaitForExitAsync(_process!).ConfigureAwait(false);

        if (_options.FailOnNegativeExitCode && exitCode != 0)
        {
            throw new Exception($"Process exit code is non-zero: {exitCode}");
        }

        var result = new CommandResult() { ExitCode = exitCode, };

        await _stdoutReader.WaitAsync().ConfigureAwait(false);
        _stdoutReader.CopyResultTo(ref result, true);

        await _stderrReader.WaitAsync().ConfigureAwait(false);
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

    private ProcessStartInfo CreateStartInfo(
        bool redirectStandardInput,
        bool redirectStandardOutput,
        bool redirectStandardError
    )
    {
        var wslPath = _io.Combine(
            _environment.GetFolderPath(Environment.SpecialFolder.System),
            "wsl.exe"
        );

        var startInfo = new ProcessStartInfo(wslPath);

        AddArguments(startInfo);

        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardInput = redirectStandardInput;
        startInfo.RedirectStandardOutput = redirectStandardOutput;
        startInfo.RedirectStandardError = redirectStandardError;

        if (redirectStandardOutput)
        {
            startInfo.StandardOutputEncoding = _options.StdoutEncoding ?? Console.OutputEncoding;
        }

        if (redirectStandardError)
        {
            startInfo.StandardErrorEncoding = _options.StderrEncoding ?? Console.OutputEncoding;
        }

        if (redirectStandardInput)
        {
            startInfo.StandardInputEncoding = _options.StdinEncoding ?? Console.InputEncoding;
        }

        return startInfo;
    }

    private void AddArguments(ProcessStartInfo startInfo)
    {
        startInfo.ArgumentList.Add("-d");
        startInfo.ArgumentList.Add(_distroName);

        if (_asRoot)
        {
            startInfo.ArgumentList.Add("--user");
            startInfo.ArgumentList.Add("root");
        }

        startInfo.ArgumentList.Add(_shellExecute ? "--" : "--exec");

        startInfo.ArgumentList.Add(_command);
        foreach (string argument in _arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }
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

    private Task<int> WaitForExitAsync(IProcess process)
    {
        if (process.HasExited)
        {
            return Task.FromResult(process.ExitCode);
        }

        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
        process.EnableRaisingEvents = true;
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
