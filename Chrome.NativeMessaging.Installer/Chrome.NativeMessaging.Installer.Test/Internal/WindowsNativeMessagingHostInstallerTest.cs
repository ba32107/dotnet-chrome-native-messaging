using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FakeItEasy;
using io.github.ba32107.Chrome.NativeMessaging.Internal;
using NUnit.Framework;

namespace io.github.ba32107.Chrome.NativeMessaging.Test.Internal
{
    internal class WindowsNativeMessagingHostInstallerTest
    {
        private const string ManifestName = "test_manifest";
        private const string RegistryKeyPrefix = @"SOFTWARE\Google\Chrome\NativeMessagingHosts";
        private const string AppDataDir = "AppDataDir";

        private WindowsNativeMessagingHostInstaller _uut;
        private IFileSystem _fs;
        private NativeMessagingHostManifest _manifest;
        private string _chromeProfileDir;
        private string _chromiumProfileDir;
        private string _chromeManifestFilePath;
        private string _chromiumManifestFilePath;

        [SetUp]
        public void SetUp()
        {
            _fs = new MockFileSystem();
            _uut = A.Fake<WindowsNativeMessagingHostInstaller>(x => x.WithArgumentsForConstructor(() =>
                    new WindowsNativeMessagingHostInstaller(_fs)
                ));

            A.CallTo(() => _uut.ResolvePath(A<string>._))
                .ReturnsLazily((string path) => path.Replace("%LOCALAPPDATA%", AppDataDir));
            A.CallTo(() => _uut.CreateRegistryKeyInCurrentUserAndSetDefaultValue(A<string>._, A<string>._))
                .DoesNothing();
            A.CallTo(() => _uut.DeleteRegistryKeyFromCurrentUserNoThrow(A<string>._)).DoesNothing();

            _manifest = new NativeMessagingHostManifest
            {
                Name = ManifestName,
                Description = "description",
                Path = "path",
                AllowedOrigins = new []
                {
                    "origin"
                }
            };

            _chromeProfileDir = _fs.Path.Combine(AppDataDir, "Google", "Chrome", "User Data");
            _chromiumProfileDir = _fs.Path.Combine(AppDataDir, "Chromium", "User Data");
            _chromeManifestFilePath = _fs.Path.Combine(_chromeProfileDir, "NativeMessagingHosts", $"{ManifestName}.json");
            _chromiumManifestFilePath = _fs.Path.Combine(_chromiumProfileDir, "NativeMessagingHosts", $"{ManifestName}.json");
        }

        [Test]
        public void TestNoBrowsersInstalledNoChangesMade()
        {
            var installedManifests = _uut.Install(_manifest);

            Assert.IsEmpty(installedManifests);
            A.CallTo(() => _uut.CreateRegistryKeyInCurrentUserAndSetDefaultValue(A<string>._, A<string>._))
                .MustNotHaveHappened();
            VerifyFileAndParentDirectoryDoesNotExist(_chromeManifestFilePath);
            VerifyFileAndParentDirectoryDoesNotExist(_chromiumManifestFilePath);
        }

        [Test]
        public void TestInstallForChrome()
        {
            _fs.Directory.CreateDirectory(_chromeProfileDir);

            var installedManifests = _uut.Install(_manifest);

            Assert.That(installedManifests.Length, Is.EqualTo(1));
            VerifyManifestInstalled(installedManifests, _chromeManifestFilePath);
            VerifyRegistryKeyCreatedForManifest(_chromeManifestFilePath);
        }

        [Test]
        public void TestInstallForChromium()
        {
            _fs.Directory.CreateDirectory(_chromiumProfileDir);

            var installedManifests = _uut.Install(_manifest);

            Assert.That(installedManifests.Length, Is.EqualTo(1));
            VerifyManifestInstalled(installedManifests, _chromiumManifestFilePath);
            VerifyRegistryKeyCreatedForManifest(_chromiumManifestFilePath);
        }

        [Test]
        public void TestInstallForBothBrowsers()
        {
            _fs.Directory.CreateDirectory(_chromeProfileDir);
            _fs.Directory.CreateDirectory(_chromiumProfileDir);

            var installedManifests = _uut.Install(_manifest);

            Assert.That(installedManifests.Length, Is.EqualTo(2));
            VerifyManifestInstalled(installedManifests, _chromeManifestFilePath);
            VerifyManifestInstalled(installedManifests, _chromiumManifestFilePath);
            VerifyRegistryKeyCreatedForManifest(_chromeManifestFilePath);
        }

        [Test]
        public void TestUninstallChromeManifest()
        {
            CreateFileWithDirectories(_chromeManifestFilePath);

            _uut.Uninstall(_manifest);

            VerifyFileAndParentDirectoryDoesNotExist(_chromeManifestFilePath);
            VerifyRegistryKeyDeleted();
        }

        [Test]
        public void TestUninstallChromiumManifest()
        {
            CreateFileWithDirectories(_chromiumManifestFilePath);

            _uut.Uninstall(_manifest);

            VerifyFileAndParentDirectoryDoesNotExist(_chromiumManifestFilePath);
            VerifyRegistryKeyDeleted();
        }

        [Test]
        public void TestUninstallBothManifests()
        {
            CreateFileWithDirectories(_chromeManifestFilePath);
            CreateFileWithDirectories(_chromiumManifestFilePath);

            _uut.Uninstall(_manifest);

            VerifyFileAndParentDirectoryDoesNotExist(_chromeManifestFilePath);
            VerifyFileAndParentDirectoryDoesNotExist(_chromiumManifestFilePath);
            VerifyRegistryKeyDeleted();
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void VerifyManifestInstalled(string[] installedManifests, string manifestPath)
        {
            Assert.True(installedManifests.Contains(manifestPath));
            Assert.True(_fs.File.Exists(manifestPath));
        }

        private void VerifyRegistryKeyCreatedForManifest(string manifestPath)
        {
            A.CallTo(() =>
                    _uut.CreateRegistryKeyInCurrentUserAndSetDefaultValue(
                        $@"{RegistryKeyPrefix}\{ManifestName}", manifestPath))
                .MustHaveHappened();
        }

        private void VerifyRegistryKeyDeleted()
        {
            A.CallTo(() =>
                _uut.DeleteRegistryKeyFromCurrentUserNoThrow(
                    $@"{RegistryKeyPrefix}\{ManifestName}")).MustHaveHappened();
        }

        private void CreateFileWithDirectories(string filePath)
        {
            _fs.Directory.CreateDirectory(_fs.Path.GetDirectoryName(filePath));
            _fs.File.Create(filePath);
        }

        private void VerifyFileAndParentDirectoryDoesNotExist(string filePath)
        {
            var parentDirectory = _fs.Path.GetDirectoryName(filePath);
            Assert.False(_fs.Directory.Exists(parentDirectory));
            Assert.False(_fs.File.Exists(filePath));
        }
    }
}