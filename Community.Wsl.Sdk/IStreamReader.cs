using System.Threading.Tasks;

namespace Community.Wsl.Sdk;

internal interface IStreamReader
{
    /// <summary>
    /// Starts copying the streamed data from the stream to an internal buffer.
    /// This is done in a separate thread.
    /// </summary>
    void Fetch();

    /// <summary>
    /// Copies the contents of the internal buffer to <see cref="CommandResult"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="isStdOut"></param>
    void CopyResultTo(ref CommandResult result, bool isStdOut);

    /// <summary>
    /// Blocks the current thread until fetching has been finished.
    /// </summary>
    void Wait();

    /// <summary>
    /// Returns a task which completes when the fetching has been finished.
    /// </summary>
    Task WaitAsync();
}
