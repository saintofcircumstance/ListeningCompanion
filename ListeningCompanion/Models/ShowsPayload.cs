using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListeningCompanion.Models
{

    public class ShowsPayload
    {
        public List<Show> ShowsOneYear { get; set; }
    }
}
