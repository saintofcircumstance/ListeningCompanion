using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanion.Models
{
    [SQLite.Table("_apiShow")]
    public class Show
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string showId { get; set; }
        public DateTime showDate { get; set; }
        public string dayName { get; set; }
        public int lengthSeconds { get; set; }
        public string venueId { get; set; }
        public string venue { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }
}
