﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanionAPIService.Models
{
    
    public class Show
    {

        public string showId { get; set; }
        public DateTime showDate { get; set; }
        public string dayName { get; set; }
        public string lengthSeconds { get; set; }
        public string venueId { get; set; }
        public string venue { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }
}
