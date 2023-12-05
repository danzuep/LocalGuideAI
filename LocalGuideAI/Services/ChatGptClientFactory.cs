using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using LocalGuideAI.Abstractions;

namespace LocalGuideAI.Services
{
    internal sealed class ChatGptClientFactory : IChatGptClientFactory
    {
        private readonly string? _apiKey;
        private readonly string? _proxyUrl;

        public ChatGptClientFactory(IConfiguration configuration)
        {
            (_apiKey, _proxyUrl) = InitializeApiValues(configuration);
        }

        static (string?, string?) InitializeApiValues(IConfiguration configuration, string sectionName = "Azure")
        {
            var section = configuration.GetSection(sectionName);
            var apiUrl = InitializeProxyUrl(section);
            var apiKey = InitializeApiKey(section);
            return (apiKey, apiUrl);
        }

        static string? InitializeApiKey(IConfiguration section, string? apiKeyValue = null, string apiKeyName = "ApiKey")
        {
            var apiKey = section[apiKeyName];
            if (!string.IsNullOrWhiteSpace(apiKey))
                apiKeyValue = apiKey;
            if (!string.IsNullOrWhiteSpace(apiKeyValue))
                StorageHelper.ApiKey = apiKeyValue;
            return apiKeyValue;
        }

        static string? InitializeProxyUrl(IConfiguration section, string? proxyUrl = "https://aoai.hacktogether.net", string apiUrlName = "ProxyUrl")
        {
            var apiUrl = section[apiUrlName];
            if (!string.IsNullOrWhiteSpace(apiUrl))
                proxyUrl = apiUrl;
            if (!string.IsNullOrWhiteSpace(proxyUrl))
                StorageHelper.ApiUrl = proxyUrl;
            return proxyUrl;
        }

        public async Task<OpenAIClient> CreateAsync()
        {
            var apiKey = await StorageHelper.GetKeyAsync() ?? _apiKey;
            var proxyUrl = await StorageHelper.GetUrlAsync() ?? _proxyUrl;
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));
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
