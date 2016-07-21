using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Counters
{
    public class Flat
    {
        [PrimaryKey, AutoIncrement]
        public int FlatId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
    }
}
