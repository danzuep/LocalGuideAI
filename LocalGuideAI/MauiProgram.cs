using Azure.AI.OpenAI;
using Azure;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.DependencyInjection;
using LocalGuideAI.Abstractions;
using LocalGuideAI.Models;
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
            // Services
            services.AddChatGptConfiguration();
            services.AddSingleton<IRecommendationService, ChatGptRecommendationService>();

            // Views and ViewModels
            services.Register<SettingsPage>();
            services.Register<PromptPage, PromptPageViewModel>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            return provider;
        }

        static IConfiguration AddChatGptConfiguration(this IServiceCollection services, string sectionName = "Azure")
        {
            var configuration = BuildConfiguration(sectionName);
            var openAiClient = configuration.CheckForValidGptApiKey(sectionName);
            services.AddSingleton(openAiClient);
            return configuration;
        }

        static IConfiguration BuildConfiguration(string? prefix)
        {
            var configurationBuilder = new ConfigurationBuilder();

            // Add configuration sources
            configurationBuilder.AddUserSecrets<App>();
            configurationBuilder.AddEnvironmentVariables(prefix);

            return configurationBuilder.Build();
        }

        static OpenAIClient CheckForValidGptApiKey(this IConfiguration configuration, string sectionName, string apiKey = "ApiKey", string proxyUrl = "https://aoai.hacktogether.net")
        {
            var section = configuration.GetSection(sectionName);
            if (string.IsNullOrWhiteSpace(section[apiKey]))
            {
                section[apiKey] = KeyHelper.StorageKey;
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                KeyHelper.StorageKey = section[apiKey];
                KeyHelper.StorageUrl = proxyUrl;
            }
            // the full key is appended by "/YOUR-GITHUB-ALIAS"
            AzureKeyCredential token = new(section[apiKey]!);
            // the full url is appended by /v1/api
            Uri proxyUri = new($"{proxyUrl}/v1/api");
            // instantiate the client with the "full" values for the url and key/token
            OpenAIClient openAiClient = new(proxyUri, token);
            return openAiClient;
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
