using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace Counters
{
    #region NavigationClasses

    public class AddCounterParameter
    {
        public int CounterId { get; set; }
        public int DataId { get; set; }
        public bool ChangeTarif { get; set; }
    }

    public class AddServiceParameter
    {
        public int ServiceId { get; set; }
        public int DataId { get; set; }
    }

    public class AddDataParameter
    {
        public int DataId { get; set; }
        public int CounterId { get; set; }
        public int ScoreId { get; set; }
    }

    public class ExportParameter
    {
        public int Type { get; set; }
        public int ScoreId { get; set; }
    }

    public class ChartSettingsParameter
    {
        public ChartSettingsParameter()
        {
            BeginDate = DateTime.Today.AddMonths(-6);
            EndDate = DateTime.Today;
        }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ChartType { get; set; }

        public string bDate { get { return BeginDate.ToString("yyyy-MM-dd HH:mm:ss"); } }
        public string eDate { get { return EndDate.ToString("yyyy-MM-dd HH:mm:ss"); } }
    }

    #endregion

    #region MenuClasses

    public class MainMenuItem : DependencyObject
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string ImgSource { get; set; }
        public string Page { get; set; }
        public string Background { get; set; }
        public bool IsLocked
        {
            get { return (bool)this.GetValue(Locked); }
            set { this.SetValue(Locked, value); }
        }
        public static readonly DependencyProperty Locked = DependencyProperty.Register("IsLocked", typeof(bool), typeof(MainMenuItem), new PropertyMetadata(false));
    }

    public class MainMenuItems : List<MainMenuItem> { }

    public class MenuItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }

    public class MenuItems : List<MenuItem> { }

    public class TypeItems : List<QueryResult> { }

    public class CheckedItem : INotifyPropertyChanged
    {
        private bool ischecked;

        public object Item { get; set; }
        public bool IsChecked
        {
            get
            {
                return ischecked;
            }
            set
            {
                ischecked = value;
                OnPropertyChanged();
            }
        }
        public double Opacity
        {
            get
            {
                return IsChecked ? 1 : 0.2;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null) return;
            handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}
