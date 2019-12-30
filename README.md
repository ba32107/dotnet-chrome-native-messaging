# Chrome Native Messaging Host libraries in .NET

A simple and intuitive .NET implementation of the [Chrome Native Messaging host](https://developer.chrome.com/apps/nativeMessaging).

* Easy to use
* No JSON parsing, no dependencies
* .NET Standard & fully platform independent
* Uses abstractions, easy to unit test
* Async support
* Separate installer library
* Available as NuGet packages

### Chrome.NativeMessaging

The main library that implements the native messaging host. It really is easy to use - all you need to implement is a `Func<string, string>`:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var response = DoSomethingAndCreateResponse(jsonMessage);
    return response;
});
```

Read [the docs](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/docs/Chrome.NativeMessaging.md) to learn more.

### Chrome.NativeMessaging.Installer

The installer package. You just need to create a manifest and let the library to the rest:
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

Read [the docs](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/docs/Chrome.NativeMessaging.Installer.md) to learn more.
