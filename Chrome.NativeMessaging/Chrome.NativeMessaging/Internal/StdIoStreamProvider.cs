using System;
using System.IO;

namespace io.github.ba32107.Chrome.NativeMessaging.Internal
{
    internal interface IStdIoStreamProvider
    {
        Stream GetStandardInputStream();
        
        Stream GetStandardOutputStream();
    }
    
    internal class StdIoStreamProvider : IStdIoStreamProvider
    {
        public Stream GetStandardInputStream()
        {
            return Console.OpenStandardInput();
        }

        public Stream GetStandardOutputStream()
        {
            return Console.OpenStandardOutput();
        }
    }
}