namespace Community.Wsl.Sdk.Strategies.Api;

public interface IIo
{
    public abstract bool Exists(string path);
    public abstract string Combine(params string[] paths);
    public abstract string GetFullPath(string path);
}
