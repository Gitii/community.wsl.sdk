namespace Community.Wsl.Sdk
{
    /// <summary>
    /// The result of the executed command in a wsl distro.
    /// </summary>
    public readonly struct CommandResult
    {
        /// <summary>
        /// The exit code of the command.
        /// </summary>
        public int ExitCode { get; init; }

        /// <summary>
        /// The data on stdout that has been received, interpreted as string.
        /// </summary>
        public string? Stdout { get; init; }

        /// <summary>
        /// The raw binary data on stdout that has been received.
        /// </summary>
        public byte[]? StdoutData { get; init; }

        /// <summary>
        /// The data on stderr that has been received, interpreted as string.
        /// </summary>
        public string? Stderr { get; init; }

        /// <summary>
        /// The raw binary data on stderr that has been received.
        /// </summary>
        public byte[]? StderrData { get; init; }
    }
}
