using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class Score
    {
        private AppSettings settings = new AppSettings();

        [PrimaryKey, AutoIncrement]
        public int ScoreId { get; set; }
        public double Data { get; set; }
        public DateTime Date { get; set; }
        public string lstServiceTarifs { get; set; }

        //Удалить
        public string lstDatas { get; set; }

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
    }
}
