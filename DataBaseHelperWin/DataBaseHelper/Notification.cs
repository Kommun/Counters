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
        public int Day { get; set; }
        public int Hour { get; set; }
        public string Message { get; set; }
        public DateTime LastDate { get; set; }

        [Ignore]
        public string Date
        {
            get
            {
                return string.Format("{0} числа, в {1}:00", Day, Hour);
            }
        }
    }
}
