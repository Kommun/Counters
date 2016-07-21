using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class ServiceTarif
    {
        [PrimaryKey, AutoIncrement]
        public int TarifId { get; set; }
        public double Tarif { get; set; }
    }
}
