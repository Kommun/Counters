using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Counters
{
    public class ServiceData
    {
        /// <summary>
        /// Id показаний услуги
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ServiceDataId { get; set; }

        /// <summary>
        /// Id услуги
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Id тарифа
        /// </summary>
        public int TarifId { get; set; }

        /// <summary>
        /// Id счета
        /// </summary>
        public int ScoreId { get; set; }
    }
}
