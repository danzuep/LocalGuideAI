using Azure.AI.OpenAI;

namespace LocalGuideAI.Abstractions
{
    internal interface IChatGptClientFactory
    {
        Task<OpenAIClient> CreateAsync();
    }
}