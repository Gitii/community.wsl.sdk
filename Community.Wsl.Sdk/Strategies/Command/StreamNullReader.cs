namespace Wslhub.Sdk.Strategies.Command;

class StreamNullReader : IStreamReader
{
    public void Fetch() { }

    public void CopyResultTo(ref CommandResult result, bool isStdOut) { }
}
