using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class Property
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
