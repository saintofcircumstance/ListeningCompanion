
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanionAPIService.Models
{
    public class Set
    {
        public string setId { get; set; }
        public int sequence { get; set; }
        public string runningLength { get; set; }
        public int runningLengthSeconds { get; set; }
        public List<PerformedSong> songs { get; set; }
    }
}
