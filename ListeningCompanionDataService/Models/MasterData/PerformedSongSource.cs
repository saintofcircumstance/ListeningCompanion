namespace ListeningCompanionDataService.Models.MasterData
{
    public class PerformedSongSource
    {
        public int ID { get; set; }
        public int PerformedSongID { get; set; }
        public int ShowID { get; set; }
        public Guid SourceUniqueID { get; set; }
        public int TrackPosition { get; set; }
        public string Title { get; set; }
        public string Mp3Url { get; set; }
        public string Mp3Mu5 { get; set; }
    }
}
