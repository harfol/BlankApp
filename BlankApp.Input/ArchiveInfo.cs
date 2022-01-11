using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Input
{
    public class ArchiveInfo : INotifyPropertyChanged
    {
        private string _no;
        public string No { get=>this._no; set { _no = value; NotifyPropertyChanged("No"); } }



        private string _title1;
        public string Title1 { get => _title1; set { _title1 = value; NotifyPropertyChanged("Title1"); } }
        private string _title2;
        public string Title2 { get => _title2; set { _title2 = value; NotifyPropertyChanged("Title2"); } }

        private string _title3;
        public string Title3 { get => _title3; set { _title3 = value; NotifyPropertyChanged("Title3"); } }



        private string _page;
        public string Page { get => _page; set { _page = value; NotifyPropertyChanged("Page"); } }

        public override string ToString()
        {
            return _no + _title1 + _title2 + _title3 + _page;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
