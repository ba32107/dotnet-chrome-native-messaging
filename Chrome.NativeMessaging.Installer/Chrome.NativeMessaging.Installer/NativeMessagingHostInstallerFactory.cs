using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using io.github.ba32107.Chrome.NativeMessaging.Internal;

namespace io.github.ba32107.Chrome.NativeMessaging
{
    public static class NativeMessagingHostInstallerFactory
    {
        /// <summary>
        /// Creates an <see cref="io.github.ba32107.Chrome.NativeMessaging.INativeMessagingHostInstaller"/> instance for the current platform.
        /// </summary>
        /// <param name="fs">An <see cref="IFileSystem"/> instance that represents the local file system</param>
        public static INativeMessagingHostInstaller CreateInstaller(IFileSystem fs)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsNativeMessagingHostInstaller(fs);
            } 
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException("Installation on Linux is not yet supported.");
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException("Installation on OS X is not yet supported.");
            }

            throw new NotSupportedException("The native messaging host installer is not supported on this platform.");
        }

    }
}