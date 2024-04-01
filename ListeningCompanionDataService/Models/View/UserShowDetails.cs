namespace ListeningCompanionDataService.Models.View
{
    public class UserShowDetails
    {
        public int UserShowID { get; set; }
        public int ShowID { get; set; }
        public string BandName { get; set; }
        public string VenueName { get; set; }
        public string Date { get; set; }
        public string InteractionStatus { get; set; }
        public bool ShowBookMarked { get; set; }
        public bool ShowLiked { get; set; } 
        public int ShowRating { get; set; }
        public string ShowNotes { get; set; }
    }
}
