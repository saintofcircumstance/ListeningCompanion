using CommunityToolkit.Maui;
using ListeningCompanionDataService.Models.View;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace ListeningCompanion
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
      
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureLifecycleEvents(events =>
                {
#if IOS
        events.AddiOS(ios => ios
            .FinishedLaunching((app, options) =>
            {
                AVFoundation.AVAudioSession session = AVFoundation.AVAudioSession.SharedInstance();
session.SetCategory(AVFoundation.AVAudioSessionCategory.Playback);
                session.SetActive(true);
                return true;
            }));
#endif
                })
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
