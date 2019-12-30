# Chrome.NativeMessaging.Installer

## Installation

Use NuGet to add the package to your project.

## Usage

### Installing the native messaging host
Installing a Chrome Native Messaging Host is an easy process: you need to create a manifest file, copy the file to the proper location, and create a registry entry (Windows only).

Learn more about the installation process on the [official documentation page](https://developer.chrome.com/apps/nativeMessaging) for native messaging.

The following code will install the same manifest as the one on the above link:
```C#
var manifest = new NativeMessagingHostManifest
{
    Name = "com.my_company.my_application",
    Description = "My Application",
    Path = @"C:\Program Files\My Application\chrome_native_messaging_host.exe",
    AllowedOrigins = new []
    {
        "knldjmfmopnpolahpmmgbagdohdnhkik"
    }
};

// System.IO.Abstractions
var fs = new FileSystem();

var installer = NativeMessagingHostInstallerFactory.CreateInstaller(fs);
installer.Install(manifest);
```
The manifest properties have the same rules as described in the official documentation. The object is validated before the installation happens. You may notice that the above code snippet does not set the `Type` property - that is because it has only one possible value, therefore it is constant.

The `Install` method returns the paths to which the manifest file was written to.

__Note:__ the installer does not copy or deploy the actual native messaging host executable. It only creates the manifest and sets it up accordingly for Chrome to find.

### Uninstalling the native messaging host

Removing the Chrome Native Messaging Host is even easier: all you need to do is delete the manifest file, and on Windows, also delete the registry key.

The code is identical to the above, but you call `Uninstall` on the installer object instead of `Install`:
```C#
installer.Uninstall(manifest);
```
__Note:__ the above action does not remove any of the native messaging host files, it only deletes the manifest and registry keys (if applicable).

### More docs
Please refer to the XML documentation for more details.

## Platform compatibility
The package is designed to be platform independent - however, the installer is currently only implemented for Windows. There are plans to implement the Linux and OS X version as well.
