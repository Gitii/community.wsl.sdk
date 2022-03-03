namespace Community.Wsl.Sdk.Strategies.Api;

public interface IEnvironment
{
    public abstract bool Is64BitOperatingSystem { get; }
    public abstract bool Is64BitProcess { get; }
    public abstract System.OperatingSystem OSVersion { get; }
    public abstract string GetFolderPath(System.Environment.SpecialFolder folder);
}
