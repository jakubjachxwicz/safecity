using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using SafeCityMobile.Common;
using SafeCityMobile.Geolocating;
using SafeCityMobile.Reporting;
using SafeCityMobile.User;
using SafeCityMobile.User.Repositories;
using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.ViewModels.Map;
using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Account.ForgotPassword;
using SafeCityMobile.Views.Map;
using SafeCityMobile.Views.Settings;
using System.Text.Json;

#if ANDROID
    using SafeCityMobile.Platforms.Android;
#endif

namespace SafeCityMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            builder.Configuration.AddJsonStream(stream);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SpaceGrotesk-Bold.ttf", "SpaceGroteskBold");
                    fonts.AddFont("SpaceGrotesk-Regular.ttf", "SpaceGroteskRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            WebViewHandler.Mapper.AppendToMapping("AllowLocal", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
#endif
            });

            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(Picker), typeof(CustomPickerHandler));
#endif
            });

            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddSingleton<Validator>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddSingleton<CurrentGeolocationService>();
            builder.Services.AddTransient<NewPasswordPage>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<ReportingPage>();
            builder.Services.AddTransient<ReportingViewModel>();
            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<VerifyCodePage>();
            builder.Services.AddTransient<SendCodePage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddSingleton<MapHelpers>();
            
            builder.Services.AddHttpClient<UserService>(httpClient =>
            {
#if ANDROID
                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("ANDROID_BASE_URL")!);
#else
                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WINDOWS_BASE_URL")!);
#endif
            });

            builder.Services.AddSingleton<IUserInfoRepository, UserInfoRestRepository>();
            builder.Services.AddHttpClient<IUserInfoRepository, UserInfoRestRepository>(httpClient =>
            {
#if ANDROID

                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("ANDROID_BASE_URL")!);
#else
                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WINDOWS_BASE_URL")!);
#endif
            });

            builder.Services.AddSingleton<IReportRepository, ReportRepository>();
            builder.Services.AddHttpClient<IReportRepository, ReportRepository>(httpClient =>
            {
#if ANDROID
                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("ANDROID_BASE_URL")!);
#else
                httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WINDOWS_BASE_URL")!);
#endif
            });

            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            builder.Services.AddTransient<AccountPageViewModel>();
            builder.Services.AddTransient<AccountPage>();
            builder.Services.AddTransient<SettingsPage>();

            return builder.Build();
        }
    }
}
