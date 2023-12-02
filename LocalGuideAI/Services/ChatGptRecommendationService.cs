using ChatGptNet;
using LocalGuideAI.Abstractions;
using System.Net;

namespace LocalGuideAI.Services
{
    internal class ChatGptRecommendationService : IRecommendationService
    {
        private readonly IChatGptClient _chatGptClient;

        public ChatGptRecommendationService(IChatGptClient chatGptClient)
        {
            _chatGptClient = chatGptClient ??
                throw new NullReferenceException("IChatGptClient missing, Builder.Services.AddChatGpt() required.");
        }

        private readonly Guid _sessionGuid = Guid.NewGuid();

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WebException"></exception>
        public async Task<string?> GetRecommendation(string prompt, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(prompt));
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                throw new WebException("No Internet access, check the network connectivity.", WebExceptionStatus.ConnectFailure);

            var response = await _chatGptClient.AskAsync(_sessionGuid, prompt, cancellationToken: cancellationToken);
            var message = response.GetContent();

            return message;
        }
    }
}
