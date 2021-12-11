namespace Wslhub.Sdk.Strategies.Command;

internal interface IStreamReader
{
    void Fetch();
    void CopyResultTo(ref CommandResult result, bool isStdOut);
}