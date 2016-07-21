using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class Tarif
    {
        [PrimaryKey, AutoIncrement]
        public int TariffId { get; set; }
        public double Tariff { get; set; }
        public bool IsSocial { get; set; }
        public double SocialNorma { get; set; }
        public double SocialTariff { get; set; }
    }
}
