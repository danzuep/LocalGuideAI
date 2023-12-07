using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using LocalGuideAI.Abstractions;

namespace LocalGuideAI.Services
{
    internal sealed class ChatGptClientFactory : IChatGptClientFactory
    {
        private readonly ValueTask _initialization;

        public ChatGptClientFactory(IConfiguration configuration)
        {
            _initialization = StorageHelper.InitializeApiValuesAsync(configuration);
        }

        public async Task<OpenAIClient> CreateAsync()
        {
            await _initialization;
            var apiKey = await StorageHelper.GetKeyAsync();
            var proxyUrl = await StorageHelper.GetUrlAsync();
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
