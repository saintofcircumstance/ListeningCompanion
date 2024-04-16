using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CommunityToolkit.Maui.Views;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion.Session
{
    public static class Session
    {
        public static int UserID { get; set; }
        public static string Username { get; set; }
        public static MediaElement AudioPlayer { get; set; } = new MediaElement() { ShouldShowPlaybackControls = true};
        public static UserSongDetails CurrentSong { get; set; }
        public static Queue<UserSongDetails> SongQueue { get; set; }

        public static void PlayNextSong()
        {
            if (SongQueue.Count > 0)
            {
                var nextSong = SongQueue.Dequeue();
                new MediaHandler().OnElementChanged(nextSong);
                CurrentSong = nextSong;
                AudioPlayer.Source = nextSong.Mp3Url;
                AudioPlayer.Play();
            }
            else
            {
                // If the queue is empty, stop playback or perform any other desired action
                AudioPlayer.Stop();
            }
        }
    }

}
