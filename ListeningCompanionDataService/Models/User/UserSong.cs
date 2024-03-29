namespace ListeningCompanionDataService.Models.User
{
    public class UserSong
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int SongID { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
        public bool Liked { get; set; }
        public bool BookMarked { get; set; }
    }
}
