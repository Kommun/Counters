using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class Service
    {
        [PrimaryKey, AutoIncrement]
        public int ServiceId { get; set; }
        public int FlatId { get; set; }
        public int TarifId { get; set; }
        public string Name { get; set; }
        public double Data { get; set; }
        public string lstCounters { get; set; }
        public int Type { get; set; }

        public int SortOrder { get; set; } = -1;
    }
}
