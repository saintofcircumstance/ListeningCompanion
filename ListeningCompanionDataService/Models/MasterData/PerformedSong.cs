namespace ListeningCompanionDataService.Models.MasterData
{
    public class PerformedSong
    {
        public int ID { get; set; }
        public int SetListID { get; set; }
        public int SongID { get; set; }
        public int SongSequence { get; set; }
        public bool Segue { get; set; }
        public int DaysSincePlayed { get; set; }
        public string LengthMMSS { get; set; }
        public int LengthSeconds { get; set; }
        public string Mp3Url { get;set; }
    }
}
