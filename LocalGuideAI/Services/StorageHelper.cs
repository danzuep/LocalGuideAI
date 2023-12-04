namespace LocalGuideAI.Services
{
    internal sealed class StorageHelper
    {
        internal static readonly string _storageKeyName = "chat_gpt_api_key";
        internal static readonly string _storageUrlName = "chat_gpt_api_url";

        internal static string? ApiKey
        {
            get => GetKeyAsync().GetAwaiter().GetResult();
            set => SetKeyAsync(value).GetAwaiter().GetResult();
        }

        internal static string? ApiUrl
        {
            get => GetUrlAsync().GetAwaiter().GetResult();
            set => SetUrlAsync(value).GetAwaiter().GetResult();
        }

        internal static async Task<string?> GetKeyAsync() =>
            await SecureStorage.Default.GetAsync(_storageKeyName);

        internal static async Task SetKeyAsync(string? value) =>
            await SecureStorage.Default.SetAsync(_storageKeyName, value ?? string.Empty);

        internal static async Task<string?> GetUrlAsync() =>
            await SecureStorage.Default.GetAsync(_storageUrlName);

        internal static async Task SetUrlAsync(string? value) =>
            await SecureStorage.Default.SetAsync(_storageUrlName, value ?? string.Empty);
    }
}
