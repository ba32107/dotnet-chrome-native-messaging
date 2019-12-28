using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using io.github.ba32107.Chrome.NativeMessaging.Internal;
using NUnit.Framework;

namespace io.github.ba32107.Chrome.NativeMessaging.Test.Internal
{
    internal class WindowsNativeMessagingHostInstallerTest
    {
        private WindowsNativeMessagingHostInstaller _uut;
        private IFileSystem _fs;
        private NativeMessagingHostManifest _manifest;

        [SetUp]
        public void SetUp()
        {
            _fs = new MockFileSystem();
            _uut = new WindowsNativeMessagingHostInstaller(_fs);

            _manifest = new NativeMessagingHostManifest();
        }

        [Test]
        public void TestInstall()
        {
            _uut.Install(_manifest);
        }
    }
}