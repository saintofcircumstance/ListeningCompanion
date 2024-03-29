namespace ListeningCompanionDataService.Models.MasterData
{
    public class Venue
    {
        public int ID { get; set; }
        public Guid VenueUniqueID { get; set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
