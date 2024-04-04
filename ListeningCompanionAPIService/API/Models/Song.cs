
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanionAPIService.Models
{
    public class Song
    {
        public string songId { get; set; }
        public string title { get; set; }
        public string firstPlayed { get; set; }
        public string firstPlayedShowId { get; set; }
        public string lastPlayed { get; set; }
        public string lastPlayedShowId { get; set; }
        public int countPlayed { get; set; }
        public int shortestSeconds { get; set; }
        public string shortestSecondsMMSS { get; set; }
        public string shortestShowId { get; set; }
        public int longestSeconds { get; set; }
        public string longestSecondsMMSS { get; set; }
        public string longestShowId { get; set; }
    }
}
