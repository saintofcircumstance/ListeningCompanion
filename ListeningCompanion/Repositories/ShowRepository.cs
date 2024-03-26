using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListeningCompanion.Repositories
{
    public class ShowRepository
    {
        string _dbPath;
        public string StatusMessage { get; set; }

        private void Init()
        {

        }

        public ShowRepository(string dbPath)
        {
            _dbPath = dbPath;
        }
    }
}
