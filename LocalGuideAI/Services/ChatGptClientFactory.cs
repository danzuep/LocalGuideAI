using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using LocalGuideAI.Abstractions;

namespace LocalGuideAI.Services
{
    internal sealed class ChatGptClientFactory : IChatGptClientFactory
    {
        private readonly IConfiguration _configuration;

        public ChatGptClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IConfiguration BuildConfiguration(string? prefix = "Azure")
        {
            var configurationBuilder = new ConfigurationBuilder();
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                // Add configuration sources
                configurationBuilder.AddUserSecrets<App>();
                configurationBuilder.AddEnvironmentVariables(prefix);
#endif
            }
            var configuration = configurationBuilder.Build();
            return configuration;
        }

        static string TestApiConfiguration(IConfiguration configuration, string? proxyUrl, string sectionName = "Azure", string apiKey = "ApiKey")
        {
            var section = configuration.GetSection(sectionName);
            if (string.IsNullOrWhiteSpace(section[apiKey]))
            {
                if (section.GetSection(apiKey).Exists())
                {
                    System.Diagnostics.Debugger.Break();
                    section[apiKey] = StorageHelper.ApiKey;
                }
            }
            else
            {
                StorageHelper.ApiKey = section[apiKey];
                StorageHelper.ApiUrl = proxyUrl;
            }
            ArgumentException.ThrowIfNullOrWhiteSpace(section[apiKey], nameof(apiKey));
            return section[apiKey]!;
        }

        public OpenAIClient Create(string? apiKey = null, string? proxyUrl = "https://aoai.hacktogether.net")
        {
            apiKey ??= TestApiConfiguration(_configuration, proxyUrl);
            // the full key is appended by "/YOUR-GITHUB-ALIAS"
            AzureKeyCredential token = new(apiKey);
            // the full url is appended by /v1/api
            Uri proxyUri = new($"{proxyUrl}/v1/api");
            // instantiate the client with the "full" values for the url and key/token
            OpenAIClient openAiClient = new(proxyUri, token);
            return openAiClient;
        }
    }
}
