using System;
using System.Collections.Generic;
using System.Text;

namespace Community.Wsl.Sdk.Strategies.Api
{
    public interface IIo
    {
        public abstract bool Exists(string path);
        public abstract string Combine(params string[] paths);
        public abstract string GetFullPath(string path);
    }
}
