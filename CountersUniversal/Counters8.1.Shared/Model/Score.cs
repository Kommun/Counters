using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Counters.Utils;

namespace Counters
{
    public class Score
    {
        private AppSettings settings = new AppSettings();

        [PrimaryKey, AutoIncrement]
        public int ScoreId { get; set; }
        public int FlatId { get; set; }
        public DateTime Date { get; set; }
        public double Recalculation { get; set; }
        public double Peni { get; set; }

        [Ignore]
        public double Data
        {
            get
            {
                var counters = QueryManager.Instance.GetCounterDataByScoreId(ScoreId);
                var services = QueryManager.Instance.GetServiceDataByScoreId(ScoreId);
                return counters.Sum(c => c.Summ + c.SummODN) + services.Sum(s => s.Summ);
            }
        }

        [Ignore]
        public string stringDate
        {
            get
            {
                return Date.ToString("MMMM yyyy");
            }
        }
        [Ignore]
        public string stringData
        {
            get
            {
                return string.Format("{0} {1}", Math.Round(Data, 2), settings.Currency);
            }
        }

        /// <summary>
        /// Устаревшее 31.03.2016
        /// </summary>
        public string lstServiceTarifs { get; set; }
    }
}
