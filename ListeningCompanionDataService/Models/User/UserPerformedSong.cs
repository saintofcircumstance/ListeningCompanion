namespace ListeningCompanionDataService.Models.User
{
    public class UserPerformedSong
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int PerformedSongID { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
        public bool Liked { get; set; }
        public bool Bookmarked { get; set; }
    }
}
