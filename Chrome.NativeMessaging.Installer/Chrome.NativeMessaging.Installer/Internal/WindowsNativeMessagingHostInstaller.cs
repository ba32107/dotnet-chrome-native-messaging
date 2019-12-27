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

            var (primaryManifestPath, allManifestPaths) = GetManifestPaths(manifest);

            allManifestPaths.ToList().ForEach(path => WriteManifestToFile(manifest, path));
            if (primaryManifestPath != null)
            {
                WriteNativeMessagingHostRegistryEntry(manifest.Name, primaryManifestPath);    
            }

            return allManifestPaths;
        }

        public void Uninstall(NativeMessagingHostManifest manifest)
        {
            ValidateManifest(manifest);

            var (_, allManifestPaths) = GetManifestPaths(manifest);

            allManifestPaths.ToList().ForEach(DeleteFileWithDirectoryIfExists);
            
            Registry.CurrentUser.DeleteSubKey(ToRegistryKeyPath(manifest.Name), false);
        }

        private static void ValidateManifest(NativeMessagingHostManifest manifest)
        {
            var ctx = new ValidationContext(manifest);
            Validator.ValidateObject(manifest, ctx, true);
        }

        private (string PrimaryManifestPath, string[] AllManifestPaths) GetManifestPaths(
            NativeMessagingHostManifest manifest)
        {
            var chromeUserDataDirectory = ResolvePath(@"%LOCALAPPDATA%\Google\Chrome\User Data");
            var chromiumUserDataDirectory = ResolvePath(@"%LOCALAPPDATA%\Chromium\User Data");
            var manifestFileRelativePath = _fs.Path.Combine("NativeMessagingHosts", $"{manifest.Name}.json");
            
            var chromeInstalled = _fs.Directory.Exists(chromeUserDataDirectory);
            var chromiumInstalled = _fs.Directory.Exists(chromiumUserDataDirectory);
            var manifestPathForChrome = _fs.Path.Combine(chromeUserDataDirectory, manifestFileRelativePath);
            var manifestPathForChromium = _fs.Path.Combine(chromiumUserDataDirectory, manifestFileRelativePath);
            
            var allManifestPaths = new List<string>();

            if (chromeInstalled)
            {
                allManifestPaths.Add(manifestPathForChrome);
            }
            if (chromiumInstalled)
            {
                allManifestPaths.Add(manifestPathForChromium);
            }

            var primaryManifestPath = chromeInstalled
                ? manifestPathForChrome
                : chromiumInstalled ? manifestPathForChromium : null;
            
            return (primaryManifestPath, allManifestPaths.ToArray());
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
        
        private static void WriteNativeMessagingHostRegistryEntry(string manifestName, string manifestPath)
        {
            var newKeyPath = ToRegistryKeyPath(manifestName);
            using (var key = Registry.CurrentUser.CreateSubKey(newKeyPath))
            {
                if (key == null)
                {
                    throw new Exception($"Unable to create or open registry key: '{newKeyPath}'");
                }
                
                key.SetValue(string.Empty, manifestPath);
            }
        }

        private void DeleteFileWithDirectoryIfExists(string filePath)
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
        
        private static string ToRegistryKeyPath(string manifestName)
        {
            return $@"SOFTWARE\Google\Chrome\NativeMessagingHosts\{manifestName}";
        }
    }
}