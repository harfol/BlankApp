namespace BlankApp.Service.Model
{
    public class Detail : NotificationObject
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _subtitle;
        public string SubTitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        private string _measure;
        public string Measure
        {
            get { return _measure; }
            set { SetProperty(ref _measure, value); }
        }

        private string _owner;
        public string Owner
        {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        private string _dossier;
        public string Dossier
        {
            get { return _dossier; }
            set { SetProperty(ref _dossier, value); }
        }
        private string _year;
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }
        private string _copies;
        public string Copies
        {
            get { return _copies; }
            set { SetProperty(ref _copies, value); }
        }

        private string _pages;
        public string Pages
        {
            get { return _pages; }
            set { SetProperty(ref _pages, value); }
        }

        private string _pageNumber;
        public string PageNumber
        {
            get { return _pageNumber; }
            set { SetProperty(ref _pageNumber, value); }
        }


        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }


        public string this[string str]
        {
            get
            {
                     if (str.Equals("Title") || str.Equals("提名")) { return this._title; }
                else if (str.Equals("SubTitle") || str.Equals("副题名")) { return this._subtitle; }
                else if (str.Equals("Measure") || str.Equals("测量编号")) { return this._measure; }
                else if (str.Equals("Owner") || str.Equals("权属人")) { return this._owner; }
                else if (str.Equals("Dossier") || str.Equals("档案号")) { return this._dossier; }
                else if (str.Equals("Number") || str.Equals("案卷号")) { return this._number; }
                else if (str.Equals("Year") || str.Equals("年份")) { return this._year; }
                else if (str.Equals("Copies") || str.Equals("份数")) { return this._copies; }
                else if (str.Equals("PageNumber") || str.Equals("页码")) { return this._pageNumber; }
                else if (str.Equals("Pages") || str.Equals("页数")) { return this._pages; }
                else if (str.Equals("Address") || str.Equals("存放地址")) { return this._address; }
                else return null;
            }
            set
            {
                     if (str.Equals("Title") || str.Equals("提名")) { this._title = value; }
                else if (str.Equals("SubTitle") || str.Equals("副题名")) {  this._subtitle = value; }
                else if (str.Equals("Measure") || str.Equals("测量编号")) {  this._measure = value; }
                else if (str.Equals("Owner") || str.Equals("权属人")) {  this._owner = value; }
                else if (str.Equals("Dossier") || str.Equals("档案号")) {  this._dossier = value; }
                else if (str.Equals("Number") || str.Equals("案卷号")) { this._number = value; }
                else if (str.Equals("Year") || str.Equals("年份")) {  this._year = value; }
                else if (str.Equals("Copies") || str.Equals("份数")) {  this._copies = value; }
                else if (str.Equals("PageNumber") || str.Equals("页码")) {  this._pageNumber = value; }
                else if (str.Equals("Pages") || str.Equals("页数")) {  this._pages = value; }
                else if (str.Equals("Address") || str.Equals("存放地址")) {  this._address = value; }
            }
        }

        public Detail Clone()
        {
            return new Detail()
            {
                Title = this._title,
                SubTitle = this._subtitle,
                Measure = this._measure,
                Owner = this._owner,
                Dossier = this._dossier,
                Number = this._number,
                Year = this._year,
                Copies = this._copies,
                PageNumber = this._pageNumber,
                Pages = this._pages,
                Address = this._address
            };
        }

    }
}
