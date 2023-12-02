using LocalGuideAI.ViewModels;

namespace LocalGuideAI.Views
{
    public partial class PromptPage : ContentPage
    {
        public PromptPage() => InitializeComponent();

        public PromptPageViewModel? ViewModel { get; private set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel = MauiProgram.GetRequiredService<PromptPageViewModel>();
            BindingContext = ViewModel;
        }
    }
}