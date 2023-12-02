using ChatGptNet;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Configuration;

namespace LocalGuideAI.Models
{
    /// <summary>
    /// Contains configuration settings for OpenAI ChatGPT services.
    /// </summary>
    /// <seealso cref="ChatGptServiceConfiguration"/>
    /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
    /// <seealso cref="AzureChatGptServiceConfiguration"/>
    internal class LlmOptions
    {
        /// <summary>
        /// Returns the <see cref="Uri"/> that provides chat completion responses.
        /// </summary>
        /// <returns>The <see cref="Uri"/> of the service.</returns>
        public Uri ApiEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the API Key to access the service.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the optional name of your Azure OpenAI Resource, leave null to use OpenAI directly instead.
        /// </summary>
        public string? AzureResourceName { get; set; }

        /// <summary>
        /// Gets or sets a value that determines the organization the user belongs to.
        /// </summary>
        /// <remarks>For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request. Usage from these API requests will count against the specified organization's subscription quota.</remarks>
        public string? Organization { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenAIChatGptServiceConfiguration"/> class.
        /// </summary>
        public LlmOptions()
        {
            ApiKey = string.Empty;
            ApiEndpoint = new("https://api.openai.com/v1/chat/completions");
        }

        private LlmOptions(IConfiguration configuration) : this()
        {
            Organization = configuration.GetValue<string>("Organization");
            ApiKey = configuration.GetValue<string>("ApiKey") ??
                throw new ArgumentException($"{nameof(ApiKey)} not set.");
            if (configuration.GetValue<Uri>("ApiEndpoint") is Uri apiEndpoint)
                ApiEndpoint = apiEndpoint;
        }

        internal static LlmOptions Create(IConfiguration configuration) =>
            new LlmOptions(configuration);

        internal static LlmOptions Create(string apiKey, string? azureResourceName = null) =>
            new LlmOptions() { ApiKey = apiKey, AzureResourceName = azureResourceName };

        internal static LlmOptions Create() =>
            Create(StorageKey ?? string.Empty);

        internal static string? StorageKey
        {
            get => GetStorageKeyAsync().GetAwaiter().GetResult();
            set => SetStorageKeyAsync(value).GetAwaiter().GetResult();
        }

        internal static async Task<string?> GetStorageKeyAsync() =>
            await SecureStorage.Default.GetAsync(_storageKeyName);

        internal static async Task SetStorageKeyAsync(string? value)
        {
            if (value != null)
                await SecureStorage.Default.SetAsync(_storageKeyName, value);
        }

        internal static readonly string _storageKeyName = "chat_gpt_api_key";

        /// <summary>
        /// Sets the configuration settings for accessing the service.
        /// </summary>
        /// <seealso cref="ChatGptServiceConfiguration"/>
        /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
        /// <seealso cref="AzureChatGptServiceConfiguration"/>
        internal void Build(ChatGptOptionsBuilder builder)
        {
            if (AzureResourceName != null)
                builder.UseAzure(AzureResourceName, ApiKey);
            else
                builder.UseOpenAI(ApiKey, Organization);
            // The following values are the default values.
            builder.DefaultModel = OpenAIChatGptModels.Gpt35Turbo;
            builder.MessageLimit = 10;
            builder.MessageExpiration = TimeSpan.FromHours(1);
        }

        /// <inheritdoc cref="Build(ChatGptOptionsBuilder)"/>
        internal static void Build(ChatGptOptionsBuilder builder, string apiKey, string? azureResourceName = null)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(apiKey));
            var llm = Create(apiKey);
            llm.Build(builder);
        }

        /// <summary>
        /// Returns the headers that are required by the service to complete the request.
        /// </summary>
        /// <returns>The collection of headers.</returns>
        public IDictionary<string, string?> GetRequestHeaders()
        {
            var headers = new Dictionary<string, string?>
            {
                ["Authorization"] = $"Bearer {ApiKey}"
            };

            if (!string.IsNullOrWhiteSpace(Organization))
            {
                headers.Add("OpenAI-Organization", Organization);
            }

            return headers;
        }

        public override string ToString() => $"{ApiEndpoint}";
    }
}
