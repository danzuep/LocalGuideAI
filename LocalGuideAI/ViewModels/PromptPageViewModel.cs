using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalGuideAI.Services;
using LocalGuideAI.Abstractions;
using LocalGuideAI.Models;

namespace LocalGuideAI.ViewModels
{
    public sealed partial class PromptPageViewModel : ObservableObject
    {
        public string Prompt
        {
            get
            {
                var daysValue = byte.TryParse(DaysEntryText, out byte days) ? days : _days;
                var locationValue = !string.IsNullOrWhiteSpace(LocationEntryText) ? LocationEntryText : _hongKong;
                return string.Format(_prompt, _itinerary, daysValue, locationValue);
            }
        }

        private string? _apiKey;
        public string? ApiKey
        {
            get => _apiKey;
            set
            {
                if (SetProperty(ref _apiKey, value))
                    LlmOptions.StorageKey = _apiKey;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Prompt))]
        private string locationEntryText = "Hong Kong";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Prompt))]
        private string daysEntryText = "2";

        [ObservableProperty]
        private string recommendationLabelText = "Click the button for recommendations!";

        private static readonly string _itinerary = "itinerary";
        private static readonly byte _days = 2;
        private static readonly string _hongKong = "Hong Kong";
        private static readonly string _prompt = "What is a recommended {0} for {1} days in {2}?";

        private readonly IRecommendationService _chatGptService;

        public PromptPageViewModel(IRecommendationService chatGptService)
        {
            LocationEntryText = _hongKong;
            DaysEntryText = _days.ToString();
            _chatGptService = chatGptService;
        }

        private bool HasCityNameAndDays =>
            !string.IsNullOrWhiteSpace(DaysEntryText) &&
            !string.IsNullOrWhiteSpace(LocationEntryText);

        [RelayCommand(CanExecute = nameof(HasCityNameAndDays))]
        private async Task GetRecommendation()
        {
            var daysValue = byte.TryParse(DaysEntryText, out byte days) ? days : _days;
            var prompt = string.Format(_prompt, _itinerary, daysValue, LocationEntryText);
            string? message = null;
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    _apiKey = await LlmOptions.GetStorageKeyAsync();
                }
                await _chatGptService.GetRecommendation(prompt);
            }
            catch (Exception ex)
            {
                await App.Current!.MainPage!.DisplayAlert("API Failure", ex.Message, "OK");
            }
            if (!string.IsNullOrEmpty(message))
                RecommendationLabelText = message;
        }
    }
}