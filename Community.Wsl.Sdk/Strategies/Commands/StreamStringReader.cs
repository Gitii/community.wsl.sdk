using System.IO;

namespace Community.Wsl.Sdk.Strategies.Commands;

internal class StreamStringReader : StreamDataReader
{
    private string? _data;

    public StreamStringReader(StreamReader reader) : base(reader) { }

    public new string? Data => _data;

    protected override void Finished(byte[] data)
    {
        _data = _reader.CurrentEncoding.GetString(data);
        base.Finished(data);
    }

    public override void CopyResultTo(ref CommandResult result, bool isStdOut)
    {
        base.CopyResultTo(ref result, isStdOut);

        if (isStdOut)
        {
            result = result with { StdoutData = null, Stdout = Data };
        }
        else
        {
            result = result with { StderrData = null, Stderr = Data };
        }
    }
}
