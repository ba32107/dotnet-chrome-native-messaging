namespace io.github.ba32107.Chrome.NativeMessaging
{
    public interface INativeMessagingHostInstaller
    {
        /// <summary>
        /// Installs the native messaging host on the local machine. This consists of serializing the manifest into
        /// a JSON file, writing this file to the correct location (see remarks), and on Windows, making the necessary
        /// registry entries.
        /// </summary>
        /// <param name="manifest">
        /// The native messaging host manifest. The object will be validated before the installation happens.
        /// </param>
        /// <returns>
        /// The file paths to which the manifest file was written to, or <c>null</c> if the installation was
        /// not successful.
        /// </returns>
        /// <remarks>
        /// On Windows, the manifest file will be written to a subdirectory called <c>"NativeMessagingHosts"</c> in the
        /// default user data directory. The default path for these is <c>"%LOCALAPPDATA%\Google\Chrome\User Data"</c>
        /// for Chrome, and <c>"%LOCALAPPDATA%\Chromium\User Data"</c> for Chromium. See more details on
        /// https://chromium.googlesource.com/chromium/src/+/master/docs/user_data_dir.md#Windows.
        /// <br/><br/>
        /// On Linux/OS X, the manifest file will be written to the
        /// "user specific" location according to the rules described on https://developer.chrome.com/apps/nativeMessaging.
        /// <br/><br/>
        /// The method will check for the presence of user data directories for both Chrome and Chromium, and will
        /// write the manifest file for each that exists. On Windows, if both exists, the registry entry will point
        /// to the one for Chrome.
        /// <br/><br/>
        /// The file will have the same name as the native messaging host (as specified in <c>manifest</c>), with a
        /// <c>".json"</c> extension. If it already exists, it will be overwritten.
        /// </remarks>
        string[] Install(NativeMessagingHostManifest manifest);
        
        /// <summary>
        /// Uninstalls the native messaging host on the local machine. This consists of deleting the manifest file from
        /// all installed locations (see <see cref="Install"/> for details), and on Windows, removing the registry
        /// entries. This method does nothing if the manifest file or the registry keys are not present.
        /// </summary>
        /// <param name="manifest">
        /// The native messaging host manifest. The object will be validated before the removal happens.
        /// </param>
        void Uninstall(NativeMessagingHostManifest manifest);
    }
}