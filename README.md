# Chrome Native Messaging Host libraries in .NET

A simple and intuitive .NET implementation of the [Chrome Native Messaging host](https://developer.chrome.com/apps/nativeMessaging).

* Dead simple
* No JSON parsing, no dependencies
* .NET Standard & fully platform independent
* Uses abstractions, easy to unit test
* Async support
* Separate installer library
* Available as NuGet packages

### Chrome.NativeMessaging

It really is dead simple to use - all you need to implement is a `Func<string, string>`:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var response = DoSomethingAndCreateResponse(jsonMessage);
    return response;
});
```

Read [the docs](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/Chrome.NativeMessaging/README.md) to learn more.

### Chrome.NativeMessaging.Installer
