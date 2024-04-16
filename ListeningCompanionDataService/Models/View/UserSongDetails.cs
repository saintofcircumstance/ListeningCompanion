namespace ListeningCompanionDataService.Models.View
{
    public class UserSongDetails
    {
        public int UserPerformedSongId { get; set; }
        public int PerformedSongId { get; set; }
        public int ShowId { get; set; }
        public int SetSequence { get; set; }
        public string SongName { get; set; }
        public int SongSequence { get; set; }
        public bool SongBookmarked { get; set; }
        public bool SongLiked { get; set; }
        public int SongRating { get; set; }
        public string SongNotes { get; set; }
        public string Mp3Url { get; set; }
    }
}
