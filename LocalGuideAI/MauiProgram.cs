using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.DependencyInjection;
using LocalGuideAI.Abstractions;
using LocalGuideAI.Services;
using LocalGuideAI.ViewModels;
using LocalGuideAI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LocalGuideAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "Material");
                    fonts.AddFont("MaterialIconsOutlined-Regular.ttf", "MaterialOutlined");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.Register();

            return builder.Build();
        }

        public static T? GetService<T>() =>
            Ioc.Default.GetService<T>();

        public static T GetRequiredService<T>() where T : notnull =>
            Ioc.Default.GetRequiredService<T>();

        static IServiceProvider Register(this IServiceCollection services)
        {
            var configuration = BuildConfiguration();

            // Services
            services.AddSingleton(configuration);
            services.AddSingleton<IChatGptClientFactory, ChatGptClientFactory>();
            services.AddSingleton<IRecommendationService, ChatGptRecommendationService>();

            // Views and ViewModels
            services.Register<SettingsPage>();
            services.Register<PromptPage, PromptPageViewModel>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            return provider;
        }

        public static IConfiguration BuildConfiguration(string? prefix = "Azure")
        {
            var configurationBuilder = new ConfigurationBuilder();
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                // Add configuration sources
                configurationBuilder.AddUserSecrets<App>();
                configurationBuilder.AddEnvironmentVariables(prefix);
#endif
            }
            var configuration = configurationBuilder.Build();
            return configuration;
        }

        static void Register<TView>(this IServiceCollection services)
            where TView : class
        {
            // View
            services.AddSingleton<TView>();
            // Route
            var type = typeof(TView);
            Routing.RegisterRoute(type.Name, type);
        }

        static void Register<TView, TViewModel>(this IServiceCollection services)
            where TView : class
            where TViewModel : class
        {
            // ViewModel
            services.AddTransient<TViewModel>();
            services.Register<TView>();
        }
    }
}
