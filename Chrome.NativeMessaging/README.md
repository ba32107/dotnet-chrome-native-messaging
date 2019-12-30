# Chrome.NativeMessaging

## Installation

Use NuGet to add the package to your project.

## Usage

If you just need a simple messaging host that responds to Chrome messages, use `StartListening`. This method will continously process incoming messages and send replies, until Chrome disconnects the port or it is closed. You need to do your own JSON serialization. The below example uses `Newtonsoft.Json` and sends the message back unaltered:
```C#
var host = new NativeMessagingHost();
host.StartListening(jsonMessage =>
{
    var message = JsonConvert.DeserializeObject(jsonMessage);
    return JsonConvert.SerializeObject(message);
});
```

If you need to detect when the connection with Chrome terminates or Chrome closes, pass in an additional `Action` that handles the disconnect event:
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

Additionally, you can send messages to Chrome at any time, independent of the listening loops by using `SendMessage` or `SendMessageAsync`:
```C#
host.Send(myMessageAsJson);
```
```C#
await host.SendAsync(myMessageAsJson);
```

#### Difference between the `StartListening` and `Send` methods

The different flavours of the `StartListening` will keep executing as long as Chrome keeps the connection open. They are intended to reply to messages coming from the Chrome extension. The interaction between Chrome and the native messaging host can only be initiated from Chrome.

The `Send` methods are the opposite. They can be used to initiate communication from the native messaging host towards Chrome. This is useful if you need to notify your Chrome extension of events that happen on the local machine.

This also means that should *__should not__* do something like this:
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
This does not make sense at all: the message handler in the listening loop only triggers when Chrome sends a message to the host, and will send the returned value back to Chrome. You don't have to (and should not) invoke `host.Send` yourself in the message handler.

The correct way to use the `Send` methods is to invoke them outside of the listening loops. However, Chrome will __only__ receive those messages if there is an open connection between Chrome and the host.

### FAQ

#### Why doesn't this library do the JSON parsing for me?
Depending on your use-case, you might have a number of different JSON schemas in your application. For example, you might have an `action` field, and depending what value the Chrome extension sends in this field, the additional JSON fields might be completely different.

Implementing a single, typed message handler is not possible with multiple JSON schemas. In order to provide the most flexibility, JSON serialization / deserialization is left up to the user.

#### Why aren't there any `StopListening` methods?
If the connection between Chrome and the host is closed from the host side (if for example the program terminates before Chrome disconnects), Chrome will mark it as a Native Messaging Host error. The host is not supposed to disconnect from Chrome and there is no clean way to do it - therefore stopping the listening loop is not implemented.

#### How exactly does each method work?
Please refer to the XML documentation.