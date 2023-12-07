using Microsoft.Extensions.Configuration;

namespace LocalGuideAI.Services
{
    internal sealed class StorageHelper
    {
        internal static readonly string _storageKeyName = "chat_gpt_api_key";
        internal static readonly string _storageUrlName = "chat_gpt_api_url";

        internal static async Task<string?> GetKeyAsync() =>
            await SecureStorage.Default.GetAsync(_storageKeyName);

        internal static async Task SetKeyAsync(string? value) =>
            await SecureStorage.Default.SetAsync(_storageKeyName, value ?? string.Empty);

        internal static async Task<string?> GetUrlAsync() =>
            await SecureStorage.Default.GetAsync(_storageUrlName);

        internal static async Task SetUrlAsync(string? value) =>
            await SecureStorage.Default.SetAsync(_storageUrlName, value ?? string.Empty);

        internal static async ValueTask InitializeApiValuesAsync(IConfiguration configuration, string sectionName = "Azure")
        {
            var section = configuration.GetSection(sectionName);
            await InitializeApiKeyAsync(section);
            await InitializeProxyUrlAsync(section);
        }

        static async ValueTask InitializeApiKeyAsync(IConfiguration section, string? apiKeyValue = null, string apiKeyName = "ApiKey")
        {
            var apiKey = section[apiKeyName];
            if (!string.IsNullOrWhiteSpace(apiKey))
                apiKeyValue = apiKey;
            if (!string.IsNullOrWhiteSpace(apiKeyValue))
                await SetKeyAsync(apiKeyValue);
        }

        static async ValueTask InitializeProxyUrlAsync(IConfiguration section, string? proxyUrl = "https://aoai.hacktogether.net", string apiUrlName = "ProxyUrl")
        {
            var apiUrl = section[apiUrlName];
            if (!string.IsNullOrWhiteSpace(apiUrl))
                proxyUrl = apiUrl;
            if (!string.IsNullOrWhiteSpace(proxyUrl))
                await SetUrlAsync(proxyUrl);
        }
    }
}
