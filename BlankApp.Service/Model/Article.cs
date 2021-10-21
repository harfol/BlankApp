using System.Collections.ObjectModel;

namespace BlankApp.Service.Model
{
    public enum ArticleTypes
    {
        Cover,
        Catalog,
        Normal,
        Group
    }
    public class Article : NotificationObject
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

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
            

        private bool _isGroup;
        public bool IsGroup
        {
            get { return _isGroup; }
            set { SetProperty(ref _isGroup, value); }
        }

        public Article()
        {

        }

        
        public Article(int id, string mask, bool isGroup, ArticleTypes type)
        {
            this._id = id;
            this._mask = mask;
            this._isGroup = isGroup;
            this.Type = type;
        }

        public ArticleTypes Type { get; set; }

        // public string[] SourceFilePaths { get; set; }
        public string ArticlePath { get; set; }
        public string[] PdfPaths { get; set; }
        public string TxtPath { get; set; }
        public Archive Parent { get; set; }

        public Detail _detail;
        public Detail Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }
    }
}
