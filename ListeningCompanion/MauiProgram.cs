using ListeningCompanion.Repositories;
using ListeningCompanionDataService.Models.View;
using Microsoft.Extensions.Logging;

namespace ListeningCompanion
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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<CurrentDayShowView>();
            builder.Services.AddTransient<UserShowDetails>();
            return builder.Build();
        }
    }
}
