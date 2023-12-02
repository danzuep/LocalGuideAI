using LocalGuideAI.Models;
using Microsoft.Extensions.Options;

namespace LocalGuideAI.Services
{
    internal class ChatGptServiceFactory
    {
        private readonly LlmOptions _options;

        public ChatGptServiceFactory(IOptions<LlmOptions>? options = null)
        {
            _options = options?.Value ?? new();
        }

        public static ChatGptServiceFactory Create(LlmOptions llmOptions)
        {
            var options = Options.Create(llmOptions);
            return new ChatGptServiceFactory(options);
        }
    }
}
