using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.github.ba32107.Chrome.NativeMessaging.Internal;

namespace io.github.ba32107.Chrome.NativeMessaging
{
    /// <summary>
    /// Default implementation of <see cref="io.github.ba32107.Chrome.NativeMessaging.INativeMessagingHost"/>. This
    /// class is not thread safe.
    /// </summary>
    public class NativeMessagingHost : INativeMessagingHost
    {
        private const int MessagePrefixLength = 4;

        private readonly IStdIoStreamProvider _streamProvider;
        private bool _listening;

        public NativeMessagingHost() : this(new StdIoStreamProvider()) { }

        internal NativeMessagingHost(IStdIoStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public void Send(string message)
        {
            var messageByteArray = MessageToChromeCompliantByteArray(message);

            using (var stdOut = _streamProvider.GetStandardOutputStream())
            {
                stdOut.Write(messageByteArray, 0, messageByteArray.Length);
            }
        }

        public async Task SendAsync(string message)
        {
            var messageByteArray = MessageToChromeCompliantByteArray(message);

            using (var stdOut = _streamProvider.GetStandardOutputStream())
            {
                await stdOut.WriteAsync(messageByteArray, 0, messageByteArray.Length);
            }
        }

        public void StartListening(Func<string, string> messageHandler)
        {
            StartListening(messageHandler, () => { });
        }

        public void StartListening(Func<string, string> messageHandler, Action disconnectHandler)
        {
            if (_listening)
            {
                return;
            }

            _listening = true;

            while (true)
            {
                using (var stdIn = _streamProvider.GetStandardInputStream())
                {
                    var messagePrefix = new byte[MessagePrefixLength];
                    stdIn.Read(messagePrefix, 0, MessagePrefixLength);
                    var messageLength = BitConverter.ToInt32(messagePrefix, 0);

                    if (messageLength == 0)
                    {
                        disconnectHandler();
                        _listening = false;
                        return;
                    }

                    var buffer = new byte[messageLength];
                    stdIn.Read(buffer, 0, messageLength);
                    var message = Encoding.UTF8.GetString(buffer);

                    var response = messageHandler(message);
                    Send(response);
                }
            }
        }

        public async Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler)
        {
            await StartListeningAsync(asyncMessageHandler, () => Task.CompletedTask);
        }

        public async Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler,
            Func<Task> asyncDisconnectHandler)
        {
            if (_listening)
            {
                return;
            }

            _listening = true;

            while (true)
            {
                using (var stdIn = _streamProvider.GetStandardInputStream())
                {
                    var messagePrefix = new byte[MessagePrefixLength];
                    await stdIn.ReadAsync(messagePrefix, 0, MessagePrefixLength);
                    var messageLength = BitConverter.ToInt32(messagePrefix, 0);

                    if (messageLength == 0)
                    {
                        await asyncDisconnectHandler();
                        _listening = false;
                        return;
                    }

                    var buffer = new byte[messageLength];
                    await stdIn.ReadAsync(buffer, 0, messageLength);
                    var message = Encoding.UTF8.GetString(buffer);

                    var response = await asyncMessageHandler(message);
                    await SendAsync(response);
                }
            }
        }

        private static byte[] MessageToChromeCompliantByteArray(string message)
        {
            var messageAsBytes = Encoding.UTF8.GetBytes(message);
            var messagePrefix = BitConverter.GetBytes(messageAsBytes.Length);
            return messagePrefix.Concat(messageAsBytes).ToArray();
        }
    }
}
