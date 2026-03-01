using Microsoft.Extensions.Logging;
using RoyalBakeryGrn.Pages;
using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

            // Register ApiClient as singleton
            builder.Services.AddSingleton<ApiClient>();

            // Register pages for DI
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddGRN>();
            builder.Services.AddTransient<AdjustmentRequestsListPage>();
            builder.Services.AddTransient<EditGRNPage>();
            builder.Services.AddTransient<ClearancePage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
