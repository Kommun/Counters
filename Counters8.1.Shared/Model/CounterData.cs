﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
    public class CounterData
    {
        /// <summary>
        /// ID показаний
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int DataId { get; set; }

        /// <summary>
        /// ID счетчика
        /// </summary>
        public int CounterId { get; set; }

        /// <summary>
        /// ID тарифа
        /// </summary>
        public int TariffId { get; set; }

        /// <summary>
        /// ID счета
        /// </summary>
        public int ScoreId { get; set; }

        /// <summary>
        /// Показания
        /// </summary>
        public double Data { get; set; }

        /// <summary>
        /// Общедомовые нужды
        /// </summary>
        public double DataODN { get; set; }

        /// <summary>
        /// Дата внесения показаний
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Начальные показания
        /// </summary>
        public bool IsFirst { get; set; }


        //устаревшие 11.03.2016
        public double Delta { get; set; }
        public double Summ { get; set; }
        public double SummODN { get; set; }
        public string stringSumm { get; set; }
    }
}
