using System;
using System.IO;
using System.Threading;

namespace Community.Wsl.Sdk.Strategies.Command
{
    internal class StreamStringReader : IStreamReader
    {
        private StreamReader _reader;
        private Thread? _thread;
        private string? _data;

        public StreamStringReader(StreamReader reader)
        {
            _reader = reader;
        }

        public string? Data => _data;

        private void Finished(string data)
        {
            _data = data;
        }

        public void Fetch()
        {
            if (_thread != null)
            {
                throw new ArgumentException("Already started fetching!");
            }

            _thread = new Thread(
                () =>
                {
                    var content = _reader.ReadToEnd();
                    Finished(content);
                }
            );

            _thread.Start();
        }

        public void CopyResultTo(ref CommandResult result, bool isStdOut)
        {
            if (_thread == null)
            {
                throw new ArgumentException("Data hasn't been fetched, yet!");
            }

            if (_thread.ThreadState != ThreadState.Stopped)
            {
                throw new ArgumentException("Fetching hasn't been finished, yet!");
            }

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
}
