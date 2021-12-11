namespace Community.Wsl.Sdk.Strategies.Command;

internal interface IStreamReader
{
    void Fetch();
    void CopyResultTo(ref CommandResult result, bool isStdOut);
}