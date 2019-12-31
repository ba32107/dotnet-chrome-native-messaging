[<img src="https://img.shields.io/nuget/v/Chrome.NativeMessaging?label=Chrome.NativeMessaging">](https://www.nuget.org/packages/Chrome.NativeMessaging/)
[<img src="https://img.shields.io/nuget/v/Chrome.NativeMessaging.Installer?label=Chrome.NativeMessaging.Installer">](https://www.nuget.org/packages/Chrome.NativeMessaging.Installer/)

# Chrome Native Messaging Host libraries in .NET
  
A simple and intuitive .NET implementation of the [Chrome Native Messaging](https://developer.chrome.com/apps/nativeMessaging) protocol.

* Easy to use
* No JSON parsing or dependencies
* .NET Standard & platform independent
* Testable through abstractions
* Async support
* Separate installer library
* NuGet packages available

### Chrome.NativeMessaging

Creating a native messaging host is dead simple: just call `StartListening` and pass in your message handler as a `Func<string, string>`:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var response = DoSomethingAndCreateResponse(jsonMessage);
    return response;
});
```

To learn more, check out [the docs](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/docs/Chrome.NativeMessaging.md) and the [Examples](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/docs/Examples.md) page.

### Chrome.NativeMessaging.Installer

Use this package to install your native messaging host. Just create a manifest and let the library do the rest:
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
