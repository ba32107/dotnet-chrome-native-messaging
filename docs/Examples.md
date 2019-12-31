# Example usage

The following guide assumes that you already have a working Chrome Extension. Building one is not in scope for this guide, please refer to the [official documentation](https://developer.chrome.com/extensions/getstarted) for details.

#### Installation
 1. Get the ID of your Chrome Extension (e.g. `knldjmfmopnpolahpmmgbagdohdnhkik`). You can find it on the Extensions page at chrome://extensions/
 1. Install the [ExampleApp](https://github.com/ba32107/dotnet-chrome-native-messaging/tree/master/ExampleApp) native messaging host:
```Shell
git clone git@github.com:ba32107/dotnet-chrome-native-messaging.git
cd dotnet-chrome-native-messaging/ExampleApp/ExampleApp
dotnet build -c Release
./bin/Release/netcoreapp3.1/ExampleApp.exe --install <chrome_extension_id>
```

#### Simple usage
 1. Call the native messaging host like so:
 ```Javascript
chrome.runtime.sendNativeMessage("io.github.ba32107.chrome.native_messaging.example_app", {
    text: "Hello World!"
}, function (response) {
    alert(response.text);
});
 ```
 2. You should see the following alert box:
 
 ![example alert box](https://raw.githubusercontent.com/ba32107/dotnet-chrome-native-messaging/example_readme/docs/alert.png)

#### Long-lived connection

In addition to sending a single message to the native messaging host, you can also create a long-lived connection:
```Javascript
port = chrome.runtime.connectNative("io.github.ba32107.chrome.native_messaging.example_app");
port.onMessage.addListener(function(message) {
    alert("Received: " + message.text);
});

port.postMessage({ text: "Connection initialized" });
```
The above example will send one message to the host, and keep listening to messages. Anything you send in the native messaging host with `Send` or `SendAsync` will arrive in the above event listener.

__Note:__ the above code snippet will keep the ExampleApp running. You should terminate it by calling `port.disconnect();`.

#### Async methods
You can use async methods in the native messaging host. The ExampleApp only contains a very basic example. Uncomment https://github.com/ba32107/dotnet-chrome-native-messaging/blob/master/ExampleApp/ExampleApp/Program.cs#L48 to try it out.

#### Cleaning up

```Shell
./bin/Release/netcoreapp3.1/ExampleApp.exe --uninstall <chrome_extension_id>
```