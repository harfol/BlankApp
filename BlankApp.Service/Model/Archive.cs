using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlankApp.Service.Model
{
    public class Archive : NotificationObject, IEditableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _mask;
        public string Mask
        {
            get { return _mask; }
            set { SetProperty(ref _mask, value); }  
        }
        public string SourceFilePath { get; set; }

        public Detail _detail;
        public Detail Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }

        private ObservableCollection<Article> _nodes;
        public ObservableCollection<Article> Nodes
        {
            get { return _nodes; }
            set { SetProperty(ref _nodes, value); }
        }

        public void BeginEdit()
        {
            ;
        }

        public void EndEdit()
        {
            ;
        }

        public void CancelEdit()
        {
            ; 
        }
    }
}
