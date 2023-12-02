using ChatGptNet;
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
            services.Register<PromptPage, PromptPageViewModel>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            return provider;
        }

        static IConfiguration AddChatGptConfiguration(this IServiceCollection services, string sectionName = "ChatGPT")
        {
            var configuration = BuildConfiguration(sectionName);
            var proxyUrl = configuration.CheckForValidGptApiKey(sectionName);
            services.AddChatGpt((services, options) =>
            {
                // Configure common properties (message limit and expiration, default parameters, ecc.)
                options.UseConfiguration(configuration, sectionName);
                //var accountService = services.GetRequiredService<IAccountService>();
                //options.UseOpenAI(apiKey);
            });
            return configuration;
        }

        static IConfiguration BuildConfiguration(string? prefix)
        {
            var configurationBuilder = new ConfigurationBuilder();

            // Add configuration sources
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configurationBuilder.AddUserSecrets<App>();
            configurationBuilder.AddEnvironmentVariables(prefix);

            return configurationBuilder.Build();
        }

        static string CheckForValidGptApiKey(this IConfiguration configuration, string sectionName, string apiKey = "ApiKey", string proxyUrl = "ProxyUrl", string provider = "Provider")
        {
            var section = configuration.GetSection(sectionName);
            if (string.IsNullOrWhiteSpace(section[provider]))
                section[provider] = sectionName;
            if (string.IsNullOrWhiteSpace(section[apiKey]))
            {
                section[apiKey] = LlmOptions.StorageKey;
                System.Diagnostics.Debugger.Break();
            }
            return section[proxyUrl] ?? "https://aoai.hacktogether.net";
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
