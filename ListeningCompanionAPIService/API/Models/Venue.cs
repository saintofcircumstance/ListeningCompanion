
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;


namespace ListeningCompanionAPIService.Models
{
    
    public class Venue
    {

        public string venueId { get; set; }
        public string venue { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }
}
