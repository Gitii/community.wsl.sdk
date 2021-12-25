namespace Community.Wsl.Sdk.Strategies.Command;

internal interface IStreamReader
{
    void Fetch();
    void CopyResultTo(ref CommandResult result, bool isStdOut);

    /// <summary>
    /// Blocks the current thread until fetching has been finished.
    /// </summary>
    void Wait();
}