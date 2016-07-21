using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Counters.Utils;

namespace Counters
{
    public class QueryResult
    {
        private AppSettings settings = new AppSettings();

        #region Поля из таблицы Counter

        public int CounterId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public bool EnableODN { get; set; }

        public string CounterType
        {
            get
            {
                switch (TypeId)
                {
                    case 1: return "Холодная вода";
                    case 2: return "Горячая вода";
                    case 3: return "Электричество";
                    case 4: return "Газ";
                    case 5: return "Отопление";
                    case 6: return "Моторесурс";
                    default: return "";
                }
            }
        }
        public string Icon
        {
            get
            {
                switch (TypeId)
                {
                    case 1:
                    case 2: return "/Images/CounterTypes/water.png";
                    case 3: return "/Images/CounterTypes/energy.png";
                    case 4: return "/Images/CounterTypes/gas.png";
                    case 5: return "/Images/CounterTypes/heat.png";
                    case 6: return "/Images/CounterTypes/moto.png";
                    default: return "";
                }
            }
        }
        public string IconColor
        {
            get
            {
                switch (TypeId)
                {
                    case 1: return "Blue";
                    case 2: return "Red";
                    case 3: return "Orange";
                    case 4: return "SkyBlue";
                    case 5: return "IndianRed";
                    case 6: return "Gray";
                    default: return "";
                }
            }
        }
        public string Unit
        {
            get
            {
                switch (TypeId)
                {
                    case 1:
                    case 2:
                    case 4: return "м3";
                    case 3: return "кВт*ч";
                    case 5: return "Гкал";
                    case 6: return "мтч";
                    default: return "";
                }
            }
        }

        #endregion

        #region Поля из таблицы CounterData

        private double? _summ;

        public int DataId { get; set; }
        public int NextDataId { get; set; }
        public double Data { get; set; }
        public double DataODN { get; set; }
        public DateTime Date { get; set; }
        public double SummODN { get; set; }
        public string stringSumm { get; set; }
        public int ScoreId { get; set; }
        public bool IsFirst { get; set; }

        public double Delta
        {
            get
            {
                if (PreviousData != null && DataId != 0)
                    return Data - PreviousData.Data;
                else
                    return 0;
            }
        }

        public double Summ
        {
            get
            {
                if (_summ == null)
                    SetSumm();
                return _summ.Value;
            }
        }

        public QueryResult PreviousData
        {
            get { return QueryManager.Instance.GetPreviousData(this); }
        }

        public string stringSummODN
        {
            get
            {
                return SummODN == 0 ? "" : string.Format("{0} {1}", SummODN, settings.Currency);
            }
        }

        public string stringFullSumm
        {
            get
            {
                return string.Format("{0} {1}", Summ, settings.Currency);
            }
        }

        public string stringDetailSumm
        {
            get
            {
                return Summ == 0 ? "" : string.Format("{0} {1}{2}", Summ, stringSumm == null || stringSumm == "" ? "" : string.Format("({0}) ", stringSumm), settings.Currency);
            }
        }

        public string stringDate
        {
            get
            {
                var strDate = Date.ToString("dd MMM yy");
                if (IsFirst)
                    strDate += " (н.п.)";

                return strDate;
            }
        }

        public string stringData
        {
            get { return string.Format("{0} {1}", Data, Unit); }
        }

        public string stringDataWithDelta
        {
            get { return string.Format("{0} ({1:+#.##;-#.##;+0}) {2}", Data, Delta, Unit); }
        }

        private void SetSumm()
        {
            var fullSumm = Calculate(Delta + DataODN, false);
            var summWithoutODN = Calculate(Delta, true);

            _summ = Math.Round(summWithoutODN, 2);
            SummODN = Math.Round(fullSumm - summWithoutODN, 2);
        }

        private double Calculate(double delta, bool saveSumm)
        {
            double summ1 = 0, summ2 = 0, summ3 = 0;
            if (delta > Limit2)
            {
                summ1 = (delta - Limit2) * Tarif3;
                delta = Limit2;
            }
            if (delta > Limit1 && TarifsCount > 1)
            {
                summ2 = (delta - Limit1) * Tarif2;
                delta = Limit1;
            }
            if (TarifsCount > 2)
                summ3 = delta * Tarif1;

            var summs = new List<double> { summ3, summ2, summ1 };
            if (saveSumm && summs.Count(s => s > 0) > 1)
                stringSumm = string.Join(" + ", summs.Where(s => s > 0).Select(s => Math.Round(s, 2).ToString()));

            return summ1 + summ2 + summ3;
        }

        #endregion

        #region Поля из таблицы Tarif

        public int TariffId { get; set; }
        public int TarifsCount { get; set; }
        public double Tarif1 { get; set; }
        public double Limit1 { get; set; }
        public double Tarif2 { get; set; }
        public double Limit2 { get; set; }
        public double Tarif3 { get; set; }

        public string stringTariff
        {
            get
            {
                switch (TarifsCount)
                {
                    case 1:
                        return string.Format("{0} {1}", Tarif3, settings.Currency);
                    case 2:
                        return string.Format("{0} / {1} {2}", Tarif2, Tarif3, settings.Currency);
                    case 3:
                        return string.Format("{0} / {1} / {2} {3}", Tarif1, Tarif2, Tarif3, settings.Currency);
                    default:
                        return "";
                }
            }
        }

        #endregion
    }
}
