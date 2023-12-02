namespace LocalGuideAI.Abstractions
{
    public interface IRecommendationService
    {
        Task<string?> GetRecommendation(string prompt, CancellationToken cancellationToken = default);
    }
}