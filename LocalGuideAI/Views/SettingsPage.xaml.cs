using LocalGuideAI.Models;
using LocalGuideAI.Services;
using System.Windows.Input;

namespace LocalGuideAI.Views
{
    public sealed partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Task.CompletedTask;
            base.OnAppearing();
            BindingContext = this;
            ApiKey = await StorageHelper.GetKeyAsync();
            ApiUrl = await StorageHelper.GetUrlAsync();
        }

        private string? _apiKey;
        public string? ApiKey
        {
            get => _apiKey;
            set
            {
                if (_apiKey != value)
                {
                    _apiKey = value;
                    OnPropertyChanged();
                    //KeyHelper.StorageKey = _apiKey;
                }
            }
        }

        private string? _apiUrl;
        public string? ApiUrl
        {
            get => _apiUrl;
            set
            {
                if (_apiUrl != value)
                {
                    _apiUrl = value;
                    OnPropertyChanged();
                    //KeyHelper.StorageUrl = _apiUrl;
                }
            }
        }

        private About _about = new();
        public About About
        {
            get => _about;
            set
            {
                if (_about != value)
                {
                    _about = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            await StorageHelper.SetKeyAsync(ApiKey);
            await StorageHelper.SetUrlAsync(ApiUrl);
            SemanticScreenReader.Announce(SaveButton.Text);
            await Shell.Current.Navigation.PopAsync();
        }
    }
}