using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalGuideAI.Abstractions;
using System.Windows.Input;

namespace LocalGuideAI.ViewModels
{
    public sealed partial class PromptPageViewModel : ObservableObject
    {
        public ICommand NavigateCommand => new Command<Type>(
            async (type) => await AppShell.Current.GoToAsync(type.Name));

        public string Prompt
        {
            get
            {
                var daysValue = byte.TryParse(DaysEntryText, out byte days) ? days : _days;
                var locationValue = !string.IsNullOrWhiteSpace(LocationEntryText) ? LocationEntryText : _hongKong;
                return string.Format(_prompt, _itinerary, daysValue, locationValue);
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Prompt))]
        private string locationEntryText = "Hong Kong";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Prompt))]
        private string daysEntryText = "2";

        [ObservableProperty]
        private string recommendationLabelText = string.Empty;

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
            try
            {
                using CancellationTokenSource cts = new();
                await foreach (var fragment in _chatGptService.GetRecommendationAsync(prompt, cts.Token))
                {
                    RecommendationLabelText += fragment;
                    OnPropertyChanged("RecommendationLabelText");
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                await App.Current!.MainPage!.DisplayAlert("API Failure", ex.Message, "OK");
            }
        }
    }
}