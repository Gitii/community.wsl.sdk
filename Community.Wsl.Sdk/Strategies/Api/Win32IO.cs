using System.IO;

namespace Community.Wsl.Sdk.Strategies.Api;

internal class Win32IO : IIo
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public string Combine(params string[] paths)
    {
        return Path.Combine(paths);
    }

    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }
}
