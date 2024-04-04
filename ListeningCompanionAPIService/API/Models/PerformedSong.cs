
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanionAPIService.Models
{
    public class PerformedSong
    {
        public int songSequence { get; set; }
        public string songId { get; set; }
        public string title { get; set; }
        public bool segue { get; set; }
        public int daysSincePlayed { get; set; }
        public string lengthMMSS { get; set; }
        public int lengthSeconds { get; set; }
    }
}
