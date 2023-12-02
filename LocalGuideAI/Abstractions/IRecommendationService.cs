namespace LocalGuideAI.Abstractions
{
    public interface IRecommendationService
    {
        Task<string?> GetRecommendation(string prompt, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> GetRecommendationAsync(string prompt, CancellationToken cancellationToken = default);
    }
}