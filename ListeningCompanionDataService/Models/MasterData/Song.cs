namespace ListeningCompanionDataService.Models.MasterData
{
    public class Song
    {
        public int ID { get; set; }
        public Guid SongUniqueID { get; set; }
        public int BandID { get; set; }
        public string Title { get; set; }
        public string FirstPlayed { get; set; }
        public Guid FirstPlayedShowUniqueID { get; set; }
        public string LastPlayed { get; set; }
        public Guid LastPlayedShowUniqueID { get; set; }
        public int CountPlayed { get; set; }
    }
}
