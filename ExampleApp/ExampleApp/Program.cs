using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace io.github.ba32107.Chrome.NativeMessaging.ExampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var mode = args[0];
            switch (mode)
            {
                case "--help":
                    PrintUsage();
                    break;
                case "--install":
                {
                    if (TryGetSecondArgument(args, out var chromeExtensionId))
                    {
                        Install(chromeExtensionId);
                    }
                    else
                    {
                        Console.WriteLine($"{mode}: missing Chrome extension ID");
                    }

                    break;
                }
                case "--uninstall":
                {
                    if (TryGetSecondArgument(args, out var chromeExtensionId))
                    {
                        Uninstall(chromeExtensionId);
                    }
                    else
                    {
                        Console.WriteLine($"{mode}: missing Chrome extension ID");
                    }

                    break;
                }
                default:
                    Start();
                    // Task.Run(async () => { await StartAsync(); }).GetAwaiter().GetResult();
                    break;
            }
        }

        private static void PrintUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("*** Chrome.NativeMessaging Example Application ***");
            sb.AppendLine();
            sb.AppendLine("Without arguments: starts listening for Chrome extension messages.");
            sb.AppendLine("Options:");
            sb.AppendLine("--install: installs the native messaging host. Needs Chrome extension ID as second argument.");
            sb.AppendLine("--uninstall: removes the native messaging host. Needs Chrome extension ID as second argument.");

            Console.Write(sb.ToString());
        }

        private static bool TryGetSecondArgument(string[] args, out string secondArgument)
        {
            var hasAtLeastTwoArguments = args.Length >= 2;
            secondArgument = hasAtLeastTwoArguments ? args[1] : null;
            return hasAtLeastTwoArguments;
        }

        private static void Install(string chromeExtensionId)
        {
            var manifest = GetManifest(chromeExtensionId);
            var fs = new FileSystem();

            var installer = NativeMessagingHostInstallerFactory.CreateInstaller(fs);
            var installedManifestPaths = installer.Install(manifest);
            installedManifestPaths
                .ToList()
                .ForEach(manifestPath => Console.WriteLine($"Writing manifest to '{manifestPath}'"));
        }

        private static void Uninstall(string chromeExtensionId)
        {
            var manifest = GetManifest(chromeExtensionId);
            var fs = new FileSystem();

            var installer = NativeMessagingHostInstallerFactory.CreateInstaller(fs);
            installer.Uninstall(manifest);
        }

        private static NativeMessagingHostManifest GetManifest(string chromeExtensionId)
        {
            return new NativeMessagingHostManifest
            {
                Name = "io.github.ba32107.chrome.native_messaging.example_app",
                Description = "Example native messaging host, demonstrating the Chrome.NativeMessaging libraries",
                Path = Process.GetCurrentProcess().MainModule?.FileName,
                AllowedOrigins = new []
                {
                    chromeExtensionId
                }
            };
        }
        private static void Start()
        {
            var host = new NativeMessagingHost();

            host.StartListening(message =>
            {
                var request = JsonConvert.DeserializeObject<Message>(message).Text;
                var response = ReverseString(request);

                return JsonConvert.SerializeObject(new Message {
                    Text = response
                });
            }, () =>
            {
                // perform some cleanup
            });
        }

        private static async Task StartAsync()
        {
            var host = new NativeMessagingHost();

            await host.StartListeningAsync(async message =>
            {
                var request = JsonConvert.DeserializeObject<Message>(message).Text;
                var response = await ComputeResponseAsync(request);

                return JsonConvert.SerializeObject(new Message {
                    Text = response
                });
            }, CleanUpAsync);
        }

        private static async Task<string> ComputeResponseAsync(string request)
        {
            // do some work
            await Task.Delay(1000);
            return ReverseString(request);
        }

        private static async Task CleanUpAsync()
        {
            // do some cleanup
            await Task.Delay(500);
        }

        private static string ReverseString(string str)
        {
            var charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    internal class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}