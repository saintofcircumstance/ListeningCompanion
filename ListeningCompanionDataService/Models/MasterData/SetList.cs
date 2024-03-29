namespace ListeningCompanionDataService.Models.MasterData
{
    public class SetList
    {
        public int ID { get; set; }
        public Guid SetListUniqueID { get; set; }
        public int ShowID { get; set; }
        public int SetSequence { get; set; }
        public string RunTime { get; set; }
        public string RunTimeSeconds { get; set; }
    }
}
