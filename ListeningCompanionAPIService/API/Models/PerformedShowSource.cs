using System;

namespace ListeningCompanionDataService.Models.MasterData
{
    public class PerformedShowSource
    {
        public int ID { get; set; }
        public int ShowID { get; set; }
        public Guid SourceUniqueID { get; set; }
        public decimal AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
        public bool IsSoundBoard { get; set; }
        public bool IsRemaster { get; set; }
        public string ShowUrl { get; set; }
    }
}
