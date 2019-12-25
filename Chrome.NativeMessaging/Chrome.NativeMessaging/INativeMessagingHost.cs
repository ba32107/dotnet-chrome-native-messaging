using System;
using System.Threading.Tasks;

namespace io.github.ba32107.Chrome.NativeMessaging
{
    public interface INativeMessagingHost
    {
        void Send(string message);
        
        Task SendAsync(string message);

        void StartListening(Func<string, string> messageHandler);
        
        void StartListening(Func<string, string> messageHandler, Action disconnectHandler);
        
        Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler);
        
        Task StartListeningAsync(Func<string, Task<string>> asyncMessageHandler, Func<Task> asyncDisconnectHandler);
    }
}