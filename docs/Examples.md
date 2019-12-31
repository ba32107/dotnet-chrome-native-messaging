# Example usage

The following guide assumes that you already have a working Chrome Extension. Building one is not in scope in this guide - please refer to the [official documentation](https://developer.chrome.com/extensions/getstarted) for details.

AlsoWe are using the [Example Application](https://github.com/ba32107/dotnet-chrome-native-messaging/tree/master/ExampleApp) in this repo.

 1. Get the ID of your Chrome Extension (you can find it on the [Extensions](chrome://extensions/) page)
 1. Install the ExampleApp native messaging host:
```Shell
git clone git@github.com:ba32107/dotnet-chrome-native-messaging.git
cd dotnet-chrome-native-messaging/ExampleApp/ExampleApp
dotnet build -c Release
./bin/Release/netcoreapp3.1/ExampleApp.exe --install <chrome_extension_id>
```
 3. Call the native messaging host like so:
 ```Javascript
chrome.runtime.sendNativeMessage("io.github.ba32107.chrome.native_messaging.example_app", {
    text: "Hello World!"
}, function (response) {
    alert(response.text);
});
 ```
 4. You should see the following alert box:
 