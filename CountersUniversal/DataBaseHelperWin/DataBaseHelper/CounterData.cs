using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class CounterData
    {
        [PrimaryKey, AutoIncrement]
        public int DataId { get; set; }
        public int CounterId { get; set; }
        public int TariffId { get; set; }
        public int ScoreId { get; set; }
        public double Data { get; set; }
        public double DataODN { get; set; }
        public DateTime Date { get; set; }
        public double Delta { get; set; }
        public double Summ { get; set; }
        public double SummODN { get; set; }
        public string stringSumm { get; set; }
    }
}
