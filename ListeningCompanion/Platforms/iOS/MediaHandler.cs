using ListeningCompanionDataService.Models.View;
using MediaPlayer;
using UIKit;

namespace ListeningCompanion
{
    public partial class MediaHandler
    {
            public partial void OnElementChanged(UserSongDetails userSongDetails)
            {


                var nowPlayingInfo = new MPNowPlayingInfo
                {
                    Title = userSongDetails.SongName,
                    Artist = "Grateful Dead",
                    Artwork = new MPMediaItemArtwork(new UIImage("appicon_big.png"))
                };
                // Add more metadata properties as needed

                // Set the now playing info
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = nowPlayingInfo;
        }
    }
}
