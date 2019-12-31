# Chrome.NativeMessaging

## Installation

Use NuGet to add the package to your project.

## Usage

If you just need a simple messaging host that responds to Chrome messages, use `StartListening`. This method will continously process incoming messages and send replies, until the Chrome extension disconnects the port or Chrome is closed. You need to do your own JSON serialization. The below example uses `Newtonsoft.Json` and sends back the message unaltered:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    return JsonConvert.SerializeObject(message);
});
```

If you need to detect when the connection with the Chrome extension terminates or Chrome closes, pass in an additional `Action` that handles the disconnect event:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    return JsonConvert.SerializeObject(message);
}, () =>
{
    // Do some cleanup. You don't need to return anything here,
    // the connection with Chrome is already closed.
});
```

If you need to do async stuff, use `StartListeningAsync`. Everything else is pretty much the same:
```C#
var host = new NativeMessagingHost();
await host.StartListeningAsync(async jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    var response = await ProcessMessageAsync(message);
    return JsonConvert.SerializeObject(response);
});
```

The async version also has a disconnect handler:
```C#
var host = new NativeMessagingHost();
await host.StartListeningAsync(async jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    var response = await ProcessMessageAsync(message);
    return JsonConvert.SerializeObject(response);
}, async () =>
{
    await CleanUpAsync();
});
```

Additionally, you can send messages to Chrome at any time, independently of the listening loops by using `SendMessage` or `SendMessageAsync`:
```C#
host.Send(myMessageAsJson);
// or
await host.SendAsync(myMessageAsJson);
```

#### Difference between the `StartListening` and `Send` methods

The different flavours of `StartListening` will keep executing as long as Chrome keeps the connection open. They are intended to listen and reply to messages coming from the Chrome extension. The interaction between Chrome and the native messaging host is always initiated by Chrome.

The `Send` methods are the opposite. They can be used to initiate communication from the native messaging host. This is useful if you need to notify your Chrome extension of events that happen on the local machine.

This also means that you should *__should not__* do something like this:
```C#
host.StartListening(jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    var response = CreateResponse();

    // Don't do this!
    host.Send(CreateSomeOtherMessage());

    return JsonConvert.SerializeObject(response);
});
```
This does not make sense at all: the value you return in the message handler  is automatically sent back to Chrome, as a reply to the received message. You don't have to (and should not) invoke `host.Send` in the message handler.

The correct way to use the `Send` methods is to invoke them outside of the listening loops (typically as a result of some event you observe on the machine). However, Chrome will __only__ receive these messages if there is an open connection between Chrome and the host. See the [Examples](https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/docs/Examples.md#long-lived-connection) page for more information.

### FAQ

#### Why doesn't this library do the JSON parsing for me?
Depending on your use-case, you might have a number of different JSON schemas in your application. For example, you might have an `action` field, and depending on what value the Chrome extension sends in this field, the additional JSON fields might be completely different.

Implementing a single, typed message handler is not possible with multiple JSON schemas. In order to provide the most flexibility, JSON serialization / deserialization is left up to you.

#### Why aren't there any `StopListening` methods?
If the connection between Chrome and the host is closed from the host side (if for example the program terminates before Chrome disconnects), Chrome will mark that as a Native Messaging Host error. The host is not supposed to disconnect from Chrome and there is no clean way to do it - therefore stopping the listening loop is not implemented.

#### How exactly does each method work?
Please refer to the XML documentation to learn more.
