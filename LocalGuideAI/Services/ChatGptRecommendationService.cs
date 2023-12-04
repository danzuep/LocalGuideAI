using Azure.AI.OpenAI;
using LocalGuideAI.Abstractions;
using System.Net;
using System.Runtime.CompilerServices;

namespace LocalGuideAI.Services
{
    internal sealed class ChatGptRecommendationService : IRecommendationService
    {
        private readonly IChatGptClientFactory _chatGptClientFactory;

        public ChatGptRecommendationService(IChatGptClientFactory chatGptClientFactory)
        {
            _chatGptClientFactory = chatGptClientFactory ?? throw new ArgumentNullException(nameof(chatGptClientFactory));
        }

        public async IAsyncEnumerable<string> GetRecommendationAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var chatCompletionsOptions = GetChatCompletionsOptions(prompt);
            var key = await StorageHelper.GetKeyAsync();
            var url = await StorageHelper.GetUrlAsync();
            var chatGptClient = _chatGptClientFactory.Create(key, url);
            var response = await chatGptClient.GetChatCompletionsStreamingAsync(chatCompletionsOptions, cancellationToken);
            await foreach (var fragment in response)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrEmpty(fragment.ContentUpdate))
                    yield return fragment.ContentUpdate;
            }
        }

        static ChatCompletionsOptions GetChatCompletionsOptions(string prompt)
        {
            ArgumentNullException.ThrowIfNull(nameof(prompt));
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                throw new WebException("No Internet access, check the network connectivity.", WebExceptionStatus.ConnectFailure);

            ChatCompletionsOptions completionOptions = new()
            {
                MaxTokens = 2048,
                Temperature = 0.7f,
                NucleusSamplingFactor = 0.95f,
                DeploymentName = "gpt-3.5-turbo"
            };

            completionOptions.Messages.Add(new ChatMessage(ChatRole.System, "You are a helpful local travel guide who wants newcomers to experience the best of what the locale has to offer."));
            completionOptions.Messages.Add(new ChatMessage(ChatRole.User, prompt));

            return completionOptions;
        }
    }
}
