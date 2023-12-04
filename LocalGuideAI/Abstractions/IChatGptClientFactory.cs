using Azure.AI.OpenAI;

namespace LocalGuideAI.Abstractions
{
    internal interface IChatGptClientFactory
    {
        OpenAIClient Create(string? apiKey = null, string? proxyUrl = "https://aoai.hacktogether.net");
    }
}