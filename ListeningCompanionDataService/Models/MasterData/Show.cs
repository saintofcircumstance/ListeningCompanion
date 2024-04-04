namespace ListeningCompanionDataService.Models.MasterData
{
    public class Show
    {
        public int ID { get; set; }
        public Guid ShowUniqueID { get; set; }
        public int BandID { get; set; }
        public int VenueID { get; set; }
        public DateTime ShowDate { get; set; }
        public string ShowDayName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string RunTime { get; set; }
    }
}
