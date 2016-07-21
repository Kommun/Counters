﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Counters.Utils;

namespace Counters
{
    public class ServiceResult
    {
        private AppSettings settings = new AppSettings();

        //Услуга
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public double Data { get; set; }
        public string lstCounters { get; set; }
        public int Type { get; set; }

        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case 0: return "/Images/ServiceTypes/fix.png";
                    case 1: return "/Images/ServiceTypes/multi.png";
                    case 2: return "/Images/ServiceTypes/waters.png";
                    case 3: return "/Images/ServiceTypes/cooler.png";
                    default: return "";
                }
            }
        }
        public string IconColor
        {
            get
            {
                switch (Type)
                {
                    case 0: return "Teal";
                    case 1: return "Tomato";
                    case 2: return "DodgerBlue";
                    case 3: return "FireBrick";
                    default: return "";
                }
            }
        }
        public double Summ
        {
            get
            {
                double s = 0;
                switch (Type)
                {
                    case 0:
                        s = Tarif;
                        break;
                    case 1:
                        s = Tarif * Data;
                        break;
                    case 2:
                        s = Tarif * QueryManager.Instance.ServiceDataSources(ScoreId, lstCounters).Sum(qr => qr.Delta);
                        break;
                    case 3:
                        s = Tarif * Data * QueryManager.Instance.ServiceDataSources(ScoreId, lstCounters).Sum(qr => qr.Delta);
                        break;
                }
                return Math.Round(s, 2);
            }
        }
        public string stringSumm
        {
            get { return string.Format("{0} {1}", Summ, settings.Currency); }
        }

        // ServiceData
        public int ServiceDataId { get; set; }
        public int ScoreId { get; set; }

        //Тариф
        public int TarifId { get; set; }
        public double Tarif { get; set; }
    }
}