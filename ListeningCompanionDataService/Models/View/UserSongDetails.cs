namespace ListeningCompanionDataService.Models.View
{
    public class UserSongDetails
    {
        public int SetSequence { get; set; }
        public string SongName { get; set; }
        public int SongSequence { get; set; }
        public bool SongBookMarked { get; set; }
        public bool SongLiked { get; set; }
        public int SongRating { get; set; }
        public string SongNotes { get; set; }
    }
}
