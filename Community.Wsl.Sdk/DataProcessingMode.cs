namespace Community.Wsl.Sdk;

/// <summary>
/// Specifies how output data should be handled.
/// </summary>
public enum DataProcessingMode
{
    /// <summary>
    /// Data is dropped (ignored).
    /// </summary>
    Drop = 0,
    Binary,
    String,
    External
}
