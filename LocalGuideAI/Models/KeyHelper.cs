namespace LocalGuideAI.Models
{
    internal class KeyHelper
    {
        internal static readonly string _storageKeyName = "chat_gpt_api_key";

        internal static string? StorageKey
        {
            get => GetKeyAsync().GetAwaiter().GetResult();
            set => SetKeyAsync(value).GetAwaiter().GetResult();
        }

        internal static async Task<string?> GetKeyAsync() =>
            await SecureStorage.Default.GetAsync(_storageKeyName);

        internal static async Task SetKeyAsync(string? value) =>
            await SecureStorage.Default.SetAsync(_storageKeyName, value ?? string.Empty);
    }
}
