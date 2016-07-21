using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastDate { get; set; }
        public bool IsRepeatable { get; set; }

        //Устаревшие с 11.07.2015
        public int Day { get; set; }
        public int Hour { get; set; }

        [Ignore]
        public string stringDate
        {
            get
            {
                if (IsRepeatable)
                    return string.Format("{0} числа, в {1}", Date.Day, Date.ToString("HH:mm"));
                else
                    return string.Format("{0}, в {1}", Date.ToString("dd MMMM yyyy"), Date.ToString("HH:mm"));
            }
        }
    }
}
