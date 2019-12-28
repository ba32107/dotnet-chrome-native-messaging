using System.ComponentModel.DataAnnotations;
using System.Linq;
using io.github.ba32107.Chrome.NativeMessaging.Internal;
using Newtonsoft.Json;

namespace io.github.ba32107.Chrome.NativeMessaging
{
    public class NativeMessagingHostManifest
    {
        private const string DefaultType = "stdio";
        private const string OriginPrefix = "chrome-extension:";
        
        /// <summary>
        /// The name of the native messaging host. This is the same name the Chrome extension can pass to
        /// <c>runtime.connectNative</c> or <c>runtime.sendNativeMessage</c>.
        /// </summary>
        /// <remarks>
        /// This name can only contain lowercase alphanumeric characters, underscores and dots. The name cannot
        /// start or end with a dot, and a dot cannot be followed by another dot. This property is required.
        /// </remarks>
        [JsonProperty("name")]
        [Required]
        [RegularExpression(@"^[a-z0-9_]+(\.[a-z0-9_]+)*$", 
            ErrorMessage = "The \"Name\" field contains invalid characters, or starts/ends with a dot, or has consecutive dots.")]
        public string Name { get; set; }
        
        /// <summary>
        /// A description of the native messaging host.
        /// </summary>
        /// <remarks>
        /// This property is required.
        /// </remarks>
        [JsonProperty("description")]
        [Required]
        public string Description { get; set; }
        
        /// <summary>
        /// The path to the native messaging host binary.
        /// </summary>
        /// <remarks>
        /// On Linux and OSX the path must be absolute. On Windows it can be relative to the directory in which the
        /// manifest file is located. The host process is started with the current directory set to the directory
        /// that contains the host binary. This property is required.
        /// </remarks>
        [JsonProperty("path")]
        [Required]
        public string Path { get; set; }
        
        /// <summary>
        /// The type of the interface used to communicate with the native messaging host. Currently there is only one
        /// possible value for this parameter: <c>stdio</c>. This value is set by default. 
        /// </summary>
        /// <remarks>
        /// The value <c>stdio</c> indicates that Chrome should use stdin and stdout to communicate with the host.
        /// This property is read-only.
        /// </remarks>
        [JsonProperty("type")]
        [Required]
        public string Type => DefaultType;

        private string[] _allowedOrigins;
        
        /// <summary>
        /// List of extension IDs that should have access to the native messaging host. 
        /// </summary>
        /// <remarks>
        /// The correct format of these strings is <c>chrome-extension://&lt;ID&gt;/</c>. If only &lt;ID&gt; is supplied,
        /// it will be converted to the correct format. Wildcards such as <c>chrome-extension://*/*</c> are not allowed.
        /// This property is required.
        /// </remarks>
        [JsonProperty("allowed_origins")]
        [Required]
        [MinLength(1)]
        [StringArrayRegularExpression(@"^chrome-extension:\/\/[^*]*\/$", 
            ErrorMessage = "At least one of the AllowedOrigins strings does not match the format 'chrome-extension://<ID>/'.")]
        public string[] AllowedOrigins
        {
            get => _allowedOrigins;
            set
            {
                _allowedOrigins = value
                    ?.Select(origin =>
                        origin == null || origin.StartsWith(OriginPrefix)
                        ? origin
                        : $"{OriginPrefix}//{origin}/")
                    .ToArray();
            }
        }
    }
}