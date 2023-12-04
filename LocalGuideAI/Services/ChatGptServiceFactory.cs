using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;

namespace LocalGuideAI.Services
{
    internal sealed class ChatGptServiceFactory
    {
        private readonly IConfiguration _configuration;

        public ChatGptServiceFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        static string TestApiConfiguration(IConfiguration configuration, string sectionName = "Azure", string apiKey = "ApiKey")
        {
            var section = configuration.GetSection(sectionName);
            if (string.IsNullOrWhiteSpace(section[apiKey]))
            {
                section[apiKey] = KeyHelper.StorageKey;
                System.Diagnostics.Debugger.Break();
            }
            ArgumentException.ThrowIfNullOrWhiteSpace(section[apiKey], nameof(apiKey));
            return section[apiKey]!;
        }

        public OpenAIClient Create(string proxyUrl = "https://aoai.hacktogether.net")
        {
            var apiKey = TestApiConfiguration(_configuration);
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
