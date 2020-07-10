using System;
using System.Threading.Tasks;

namespace io.github.ba32107.Chrome.NativeMessaging
{
    /// <summary>
    /// Represents the native messaging host. Can be used to send a single message to Chrome, or to listen and
    /// reply to messages continuously.fdfdff
    /// </summary>
    public interface INativeMessagingHost
    {
        /// <summary>
        /// Sends a single message to Chrome.
        /// </summary>
        /// <param name="message">The message. Should be a serialized JSON.</param>
        void Send(string message);

        /// <summary>
        /// Sends a single message to Chrome asynchronously.
        /// </summary>
        /// <param name="message">The message. Should be a serialized JSON.</param>
        Task SendAsync(string message);

        /// <summary>
        /// Starts listening to incoming messages and sends replies. If the Chrome extension uses <c>chrome.runtime.sendNativeMessage</c>
        /// to send a single message, <c>messageHandler</c> will only execute once. If the Chrome extension creates a
        /// connection using <c>chrome.runtime.connectNative</c>, this method will block and keep processing messages
        /// until <c>port.disconnect()</c> is called in the extension or Chrome is closed.
        /// </summary>
        /// <param name="messageHandler">
        /// A function that takes an incoming message and responds with a reply. The message is a serialized JSON,
        /// and the reply should be the same.
        /// </param>
        /// <remarks>
        /// After the first call of this method, subsequent calls to <c>StartListening</c> or <c>StartListeningAsync</c>
        /// have no effect until the connection with Chrome is open. If the Chrome extension disconnects the port,
        /// this object can be put into listening mode again by calling one of the previously mentioned methods.
        /// </remarks>
        void StartListening(Func<string, string> messageHandler);

        /// <summary>
        /// Starts listening to incoming messages and sends replies. If the Chrome extension uses <c>chrome.runtime.sendNativeMessage</c>
        /// to send a single message, <c>messageHandler</c> will only execute once. If the Chrome extension creates a
        /// connection using <c>chrome.runtime.connectNative</c>, this method will block and keep processing messages
        /// until <c>port.disconnect()</c> is called in the extension or Chrome is closed.
        /// </summary>
        /// <param name="messageHandler">
        /// A function that takes an incoming message and responds with a reply. The message is a serialized JSON,
        /// and the reply should be the same.
        /// </param>
        /// <param name="disconnectHandler">
        /// An action that is invoked when the Chrome extension disconnects the <c>port</c> or Chrome is closed. Can
        /// be used for cleanup.
        /// </param>
        /// /// <remarks>
        /// After the first call of this method, subsequent calls to <c>StartListening</c> or <c>StartListeningAsync</c>
        /// have no effect until the connection with Chrome is open. If the Chrome extension disconnects the port,
        /// this object can be put into listening mode again by calling one of the previously mentioned methods.
        /// </remarks>
        void StartListening(Func<string, string> messageHandler, Action disconnectHandler);

        /// <summary>
        /// Starts listening to incoming messages and sends replies. If the Chrome extension uses <c>chrome.runtime.sendNativeMessage</c>
        /// to send a single message, <c>messageHandler</c> will only execute once. If the Chrome extension creates a
        /// connection using <c>chrome.runtime.connectNative</c>, this method will block and keep processing messages
        /// until <c>port.disconnect()</c> is called in the extension or Chrome is closed.
        /// </summary>
        /// <param name="asyncMessageHandler">
        /// An async function that takes an incoming message and responds with a reply. The message is a serialized JSON,
        /// and the reply should be the same.
        /// </param>
        /// /// <remarks>
        /// After the first call of this method, subsequent calls to <c>StartListening</c> or <c>StartListeningAsync</c>
        /// have no effect until the connection with Chrome is open. If the Chrome extension disconnects the port,
        /// this object can be put into listening mode again by calling one of the previously mentioned methods.
        /// </remarks>
        Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler);

        /// <summary>
        /// Starts listening to incoming messages and sends replies. If the Chrome extension uses <c>chrome.runtime.sendNativeMessage</c>
        /// to send a single message, <c>messageHandler</c> will only execute once. If the Chrome extension creates a
        /// connection using <c>chrome.runtime.connectNative</c>, this method will block and keep processing messages
        /// until <c>port.disconnect()</c> is called in the extension or Chrome is closed.
        /// </summary>
        /// <param name="asyncMessageHandler">
        /// An async function that takes an incoming message and responds with a reply. The message is a serialized JSON,
        /// and the reply should be the same.
        /// </param>
        /// <param name="asyncDisconnectHandler">
        /// An async action that is invoked when the Chrome extension disconnects the <c>port</c> or Chrome is closed.
        /// Can be used for cleanup.
        /// </param>
        /// /// <remarks>
        /// After the first call of this method, subsequent calls to <c>StartListening</c> or <c>StartListeningAsync</c>
        /// have no effect until the connection with Chrome is open. If the Chrome extension disconnects the port,
        /// this object can be put into listening mode again by calling one of the previously mentioned methods.
        /// </remarks>
        Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler, Func<Task> asyncDisconnectHandler);
    }
}