using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace io.github.ba32107.Chrome.NativeMessaging.Internal
{
    internal class WindowsNativeMessagingHostInstaller : INativeMessagingHostInstaller
    {
        private readonly IFileSystem _fs;

        internal WindowsNativeMessagingHostInstaller(IFileSystem fs)
        {
            _fs = fs;
        }

        public string[] Install(NativeMessagingHostManifest manifest)
        {
            ValidateManifest(manifest);

            var (primaryManifestPath, allManifestPaths) = GetPrimaryAndAllPossibleManifestPaths(manifest);

            allManifestPaths.ToList().ForEach(path => WriteManifestToFile(manifest, path));
            if (primaryManifestPath != null)
            {
                var registryKeyPath = ToChromeNativeMessagingHostRegistryKeyPath(manifest.Name);
                CreateRegistryKeyInCurrentUserAndSetDefaultValue(registryKeyPath, primaryManifestPath);
            }

            return allManifestPaths;
        }

        public void Uninstall(NativeMessagingHostManifest manifest)
        {
            ValidateManifest(manifest);

            var (_, allManifestPaths) = GetPrimaryAndAllPossibleManifestPaths(manifest);

            allManifestPaths.ToList().ForEach(DeleteFileWithParentDirectoryIfExists);

            var registryKeyPath = ToChromeNativeMessagingHostRegistryKeyPath(manifest.Name);
            DeleteRegistryKeyFromCurrentUserNoThrow(registryKeyPath);
        }

        private static void ValidateManifest(NativeMessagingHostManifest manifest)
        {
            Validator.ValidateObject(manifest, new ValidationContext(manifest), true);
        }

        private (string PrimaryManifestPath, string[] AllManifestPaths) GetPrimaryAndAllPossibleManifestPaths(
            NativeMessagingHostManifest manifest)
        {
            var chromeUserDataDirectory = ResolvePath(@"%LOCALAPPDATA%\Google\Chrome\User Data");
            var chromiumUserDataDirectory = ResolvePath(@"%LOCALAPPDATA%\Chromium\User Data");
            var manifestFileRelativePath = _fs.Path.Combine("NativeMessagingHosts", $"{manifest.Name}.json");

            var chromeInstalled = _fs.Directory.Exists(chromeUserDataDirectory);
            var chromiumInstalled = _fs.Directory.Exists(chromiumUserDataDirectory);
            var manifestPathForChrome = _fs.Path.Combine(chromeUserDataDirectory, manifestFileRelativePath);
            var manifestPathForChromium = _fs.Path.Combine(chromiumUserDataDirectory, manifestFileRelativePath);

            var allPossibleManifestPaths = new List<string>();

            if (chromeInstalled)
            {
                allPossibleManifestPaths.Add(manifestPathForChrome);
            }
            if (chromiumInstalled)
            {
                allPossibleManifestPaths.Add(manifestPathForChromium);
            }

            var primaryManifestPath = chromeInstalled
                ? manifestPathForChrome
                : chromiumInstalled ? manifestPathForChromium : null;

            return (primaryManifestPath, allPossibleManifestPaths.ToArray());
        }

        private string ResolvePath(string path)
        {
            return _fs.Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
        }

        private void WriteManifestToFile(NativeMessagingHostManifest manifest, string manifestFilePath)
        {
            var manifestAsJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            _fs.Directory.CreateDirectory(_fs.Path.GetDirectoryName(manifestFilePath));
            _fs.File.WriteAllText(manifestFilePath, manifestAsJson);
        }
        private void DeleteFileWithParentDirectoryIfExists(string filePath)
        {
            if (_fs.File.Exists(filePath))
            {
                var parentDirectory = _fs.Path.GetDirectoryName(filePath);
                _fs.File.Delete(filePath);
                if (!_fs.Directory.EnumerateFiles(parentDirectory).Any())
                {
                    _fs.Directory.Delete(parentDirectory);
                }
            }
        }

        private static string ToChromeNativeMessagingHostRegistryKeyPath(string manifestName)
        {
            return $@"SOFTWARE\Google\Chrome\NativeMessagingHosts\{manifestName}";
        }

        internal virtual void CreateRegistryKeyInCurrentUserAndSetDefaultValue(string key, string value)
        {
            using (var registryKey = Registry.CurrentUser.CreateSubKey(key))
            {
                if (registryKey == null)
                {
                    throw new Exception($"Unable to create or open registry key: '{key}'");
                }

                registryKey.SetValue(string.Empty, value);
            }
        }

        internal virtual void DeleteRegistryKeyFromCurrentUserNoThrow(string subKey)
        {
            Registry.CurrentUser.DeleteSubKey(subKey, false);
        }
    }
}