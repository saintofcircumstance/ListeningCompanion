using ListeningCompanion.Repositories;
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

            string dbPath = FileAccessHelper.GetLocalFilePath("listeningCompnanion.db3");
            builder.Services.AddSingleton<ShowRepository>(s => ActivatorUtilities.CreateInstance<ShowRepository>(s, dbPath));
            return builder.Build();
        }
    }
}
