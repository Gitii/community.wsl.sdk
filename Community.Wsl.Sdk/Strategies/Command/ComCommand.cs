using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Community.Wsl.Sdk.Strategies.NativeMethods;
using Microsoft.Win32.SafeHandles;

namespace Community.Wsl.Sdk.Strategies.Command
{
    public class ComCommand : ICommand
    {
        private NativePipe _stdinHandle;
        private NativePipe _stdoutHandle;
        private NativePipe _stderrHandle;
        private IntPtr? _childProcess;

        private bool _isStarted = false;
        private bool _hasWaited = false;
        private bool _isDisposed = false;
        private CommandExecutionOptions _options;
        private string _distroName;
        private string _commandLine;

        private BaseNativeMethods _nativeMethods;

        public ComCommand(
            string distroName,
            string commandLine,
            CommandExecutionOptions options,
            BaseNativeMethods? nativeMethods = null
        )
        {
            _distroName = distroName ?? throw new ArgumentNullException(nameof(distroName));
            _commandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));
            _options = options;
            _nativeMethods = nativeMethods ?? new Win32NativeMethods();
        }

        public bool IsStarted => _isStarted;

        public bool HasWaited => _hasWaited;

        public bool IsDisposed => _isDisposed;

        public bool HasExited
        {
            get
            {
                if (!_isStarted)
                {
                    throw new Exception("The command hasn't been started, yet!");
                }

                if (_hasWaited)
                {
                    return true;
                }

                return _nativeMethods.GetExitCodeProcess(
                        _childProcess
                            ?? throw new Exception(
                                "The command has been started, but the child process is null"
                            ),
                        out var exitCode
                    )
                    && exitCode != BaseNativeMethods.STILL_ACTIVE;
            }
        }

        public CommandStreams Start()
        {
            if (IsStarted)
            {
                throw new ArgumentException("Command has already been started!");
            }

            _isStarted = true;

            try
            {
                var isRegistered = _nativeMethods.WslIsDistributionRegistered(_distroName);

                if (!isRegistered)
                    throw new Exception($"{_distroName} is not registered distro.");

                (_stdinHandle, _stdoutHandle, _stderrHandle) = CreateHandles(_options);

                CreateWritersAndReaders(
                    _options,
                    ref _stdinHandle,
                    ref _stdoutHandle,
                    ref _stderrHandle
                );

                IntPtr child = IntPtr.Zero;

                var hr = _nativeMethods.WslLaunch(
                    _distroName,
                    _commandLine,
                    false,
                    _stdinHandle.ReadHandle
                        ?? throw new ArgumentNullException("stdin handle must not be null"),
                    _stdoutHandle.WriteHandle
                        ?? throw new ArgumentNullException("stdout handle must not be null"),
                    _stderrHandle.WriteHandle
                        ?? throw new ArgumentNullException("stderr handle must not be null"),
                    out child
                );

                if (hr < 0)
                    throw new COMException("Cannot launch WSL process", hr);

                _childProcess = child;

                return new CommandStreams()
                {
                    StandardInput = _stdinHandle.Writer,
                    StandardOutput = _stdoutHandle.Reader,
                    StandardError = _stderrHandle.Reader
                };
            }
            catch (Exception e)
            {
                Dispose();
                throw;
            }
        }

        public CommandResult WaitAndGetResults()
        {
            if (!IsStarted)
            {
                throw new ArgumentException("Command hasn't been started, yet!");
            }

            if (!_childProcess.HasValue)
            {
                throw new ArgumentException(
                    "Command has been started but process couldn't been started."
                );
            }

            if (HasWaited)
            {
                throw new Exception("Already waited for the command to finish!");
            }

            _hasWaited = true;

            if (_stdinHandle.WriteHandle != null)
            {
                _nativeMethods.CloseHandle(_stdinHandle.WriteHandle);
            }

            _nativeMethods.WaitForSingleObject(_childProcess.Value, BaseNativeMethods.INFINITE);

            if (!_nativeMethods.GetExitCodeProcess(_childProcess.Value, out int exitCode))
            {
                var lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError, "Cannot query exit code of the process.");
            }

            if (exitCode != 0 && _options.FailOnNegativeExitCode)
            {
                throw new Exception($"Process exit code is non-zero: {exitCode}");
            }

            var result = HandleStreams(_options, _stdoutHandle, _stderrHandle);

            return result with
            {
                ExitCode = exitCode
            };
        }

        public Task<CommandResult> WaitAndGetResultsAsync()
        {
            throw new NotSupportedException();
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

        private void CreatePipe(
            out SafeFileHandle readPipe,
            out SafeFileHandle writePipe,
            BaseNativeMethods.SECURITY_ATTRIBUTES attributes
        )
        {
            attributes.nLength = Marshal.SizeOf(attributes);

            bool success = _nativeMethods.CreatePipe(out readPipe, out writePipe, in attributes, 0);
            if (!success || readPipe.IsInvalid || writePipe.IsInvalid)
            {
                throw new Win32Exception("Failed to create Pipe!");
            }
        }

        private (SafeFileHandle read, SafeFileHandle write) CreatePipe(bool parentInputs)
        {
            BaseNativeMethods.SECURITY_ATTRIBUTES securityAttributesParent =
                new BaseNativeMethods.SECURITY_ATTRIBUTES();
            securityAttributesParent.bInheritHandle = true;

            SafeFileHandle hTmp = null;
            SafeFileHandle childHandle = null;
            SafeFileHandle parentHandle = null;
            try
            {
                if (parentInputs)
                {
                    CreatePipe(out childHandle, out hTmp, securityAttributesParent);
                }
                else
                {
                    CreatePipe(out hTmp, out childHandle, securityAttributesParent);
                }

                if (
                    !_nativeMethods.DuplicateHandle(
                        new SafeFileHandle(_nativeMethods.GetCurrentProcess(), false),
                        hTmp,
                        new SafeFileHandle(_nativeMethods.GetCurrentProcess(), false),
                        out parentHandle,
                        0,
                        false,
                        BaseNativeMethods.DuplicateOptions.DUPLICATE_SAME_ACCESS
                    )
                )
                {
                    throw new Win32Exception("Failed to duplicate handle!");
                }

                return (parentHandle, childHandle);
            }
            finally
            {
                if (hTmp != null && !hTmp.IsInvalid)
                {
                    hTmp.Close();
                }
            }
        }

        private (NativePipe stdin, NativePipe stdout, NativePipe stderr) CreateHandles(
            CommandExecutionOptions options
        )
        {
            NativePipe stdin;
            NativePipe stdout;
            NativePipe stderr;

            if (options.StdInDataProcessingMode == DataProcessingMode.Drop)
            {
                stdin = new NativePipe(
                    new SafeFileHandle(
                        _nativeMethods.GetStdHandle(BaseNativeMethods.STD_INPUT_HANDLE),
                        false
                    )
                );
            }
            else
            {
                SafeFileHandle childHandle;
                SafeFileHandle parentHandle;
                BaseNativeMethods.SECURITY_ATTRIBUTES securityAttributesParent =
                    new BaseNativeMethods.SECURITY_ATTRIBUTES();
                securityAttributesParent.bInheritHandle = false;
                CreatePipe(out childHandle, out parentHandle, securityAttributesParent);

                stdin = new NativePipe(childHandle, parentHandle);
            }

            if (options.StdoutDataProcessingMode == DataProcessingMode.Drop)
            {
                stdout = new NativePipe(
                    writeHandle: new SafeFileHandle(
                        _nativeMethods.GetStdHandle(BaseNativeMethods.STD_OUTPUT_HANDLE),
                        false
                    )
                );
            }
            else
            {
                stdout = new NativePipe(CreatePipe(false));
            }

            if (options.StdErrDataProcessingMode == DataProcessingMode.Drop)
            {
                stderr = new NativePipe(
                    writeHandle: new SafeFileHandle(
                        _nativeMethods.GetStdHandle(BaseNativeMethods.STD_ERROR_HANDLE),
                        false
                    )
                );
            }
            else
            {
                stderr = new NativePipe(CreatePipe(false));
            }

            return (stdin, stdout, stderr);
        }

        private CommandResult HandleStreams(
            CommandExecutionOptions options,
            NativePipe stdoutReader,
            NativePipe stderrReader
        )
        {
            if (options.StdoutDataProcessingMode != DataProcessingMode.Drop)
            {
                _nativeMethods.CloseHandle(_stdoutHandle.WriteHandle!);
            }

            ReadData(
                options.StdoutDataProcessingMode,
                stdoutReader.Reader,
                out var stdOutStringData,
                out var stdOutRawData
            );

            if (options.StdErrDataProcessingMode != DataProcessingMode.Drop)
            {
                _nativeMethods.CloseHandle(_stderrHandle.WriteHandle);
            }

            ReadData(
                options.StdErrDataProcessingMode,
                stderrReader.Reader,
                out var stdErrStringData,
                out var stdErrRawData
            );

            return new CommandResult()
            {
                StderrData = stdErrRawData,
                Stderr = stdErrStringData,
                Stdout = stdOutStringData,
                StdoutData = stdOutRawData
            };
        }

        private void ReadData(
            DataProcessingMode processingMode,
            StreamReader reader,
            out string? strData,
            out byte[]? rawData
        )
        {
            if (processingMode == DataProcessingMode.Binary)
            {
                var buffer = new MemoryStream();

                reader.BaseStream.CopyTo(buffer);

                strData = null;
                rawData = buffer.ToArray();
            }
            else if (processingMode == DataProcessingMode.String)
            {
                strData = reader.ReadToEnd();
                rawData = null;
            }
            else
            {
                strData = null;
                rawData = null;
            }
        }

        private static void CreateWritersAndReaders(
            CommandExecutionOptions options,
            ref NativePipe stdin,
            ref NativePipe stdout,
            ref NativePipe stderr
        )
        {
            StreamWriter stdinWriter;
            if (options.StdInDataProcessingMode == DataProcessingMode.Drop)
            {
                stdinWriter = StreamWriter.Null;
            }
            else
            {
                stdinWriter = new StreamWriter(
                    new FileStream(
                        stdin.WriteHandle
                            ?? throw new ArgumentNullException(
                                "stdin is not dropped but handle is null!"
                            ),
                        FileAccess.Write,
                        4096,
                        false
                    ),
                    Console.InputEncoding,
                    4096,
                    true
                );
                stdinWriter.AutoFlush = true;
            }

            StreamReader stdoutReader;
            if (options.StdoutDataProcessingMode == DataProcessingMode.Drop)
            {
                stdoutReader = StreamReader.Null;
            }
            else
            {
                stdoutReader = new StreamReader(
                    new FileStream(
                        stdout.ReadHandle
                            ?? throw new ArgumentNullException(
                                "stdout is not dropped but handle is null!"
                            ),
                        FileAccess.Read,
                        4096,
                        false
                    ),
                    options.StdoutEncoding ?? Console.OutputEncoding,
                    true,
                    4096,
                    true
                );
            }

            StreamReader stderrReader;
            if (options.StdErrDataProcessingMode == DataProcessingMode.Drop)
            {
                stderrReader = StreamReader.Null;
            }
            else
            {
                stderrReader = new StreamReader(
                    new FileStream(
                        stderr.ReadHandle
                            ?? throw new ArgumentNullException(
                                "stderr is not dropped but handle is null!"
                            ),
                        FileAccess.Read,
                        4096,
                        false
                    ),
                    options.StderrEncoding ?? Console.OutputEncoding,
                    true,
                    4096,
                    true
                );
            }

            stdin = stdin with { Writer = stdinWriter };

            stdout = stdout with { Reader = stdoutReader };

            stderr = stderr with { Reader = stderrReader };
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _stdinHandle.Dispose();
            _stdoutHandle.Dispose();
            _stderrHandle.Dispose();

            GC.SuppressFinalize(this);
        }

        ~ComCommand()
        {
            Dispose();
        }
    }
}
