namespace ListeningCompanionDataService.Models.User
{
    public class UserShow
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ShowID { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
        public string InteractionStatus { get; set; }
        public bool Liked { get; set; }
        public bool BookMarked { get; set; }
    }
}
