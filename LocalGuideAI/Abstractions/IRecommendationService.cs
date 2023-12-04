namespace LocalGuideAI.Abstractions
{
    public interface IRecommendationService
    {
        IAsyncEnumerable<string> GetRecommendationAsync(string prompt, CancellationToken cancellationToken = default);
    }
}