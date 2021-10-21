using BlankApp.Service.Model;
using BlankApp.Services;
using BlankApp.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Syncfusion.UI.Xaml.NavigationDrawer;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace BlankApp.ViewModel
{
    public class ArchivesInfoViewModel : BindableBase
    {
        private ObservableCollection<Archive> _archivesDetails = new ObservableCollection<Archive>();
        public ObservableCollection<Archive> ArchivesDetails
        {
            get { return _archivesDetails; }
            set { SetProperty(ref _archivesDetails, value); }
        }

        private string[] _searchFields;
        public string[] SearchFields
        {
            get { return _searchFields; }
            set { SetProperty(ref _searchFields, value); }
        }
        private string[] _filterLevel;
        public string[] FilterLevel
        {
            get { return _filterLevel; }
            set { SetProperty(ref _filterLevel, value); }
        }


        private IArchiveService _archivesService;

        public IArchiveService ArchivesService
        {
            get { return _archivesService; }
            set { _archivesService = value; }
        }


        public ArchivesInfoViewModel()
        {
            List<string> fields = Array.ConvertAll<PropertyInfo, string>(typeof(Detail).GetProperties(), p => p.Name).ToList();
            fields.Insert(0, "AllColumns");
            fields.RemoveAt(fields.Count - 1);
            SearchFields = fields.ToArray();
        }

        #region Command
        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new DelegateCommand<object>(Copy);
                }

                return _copyCommand;
            }
        }

        private void Copy(object obj)
        {
            if( obj is TreeGridNodeContextMenuInfo menuInfo)
            {
                object[] selectedItems = menuInfo.TreeGrid.SelectedItems.ToArray();
                
            }
            throw new NotImplementedException();
        }

        private ICommand _buildCommand;
        public ICommand BuildCommand
        {
            get
            {
                if (_buildCommand == null)
                {
                    _buildCommand = new DelegateCommand<object>(Build);
                }

                return _buildCommand;
            }
        }
        private void Build(object obj)
        {
            if (obj is TreeGridNodeContextMenuInfo menuInfo)
            {
                object[] selectedItems = menuInfo.TreeGrid.SelectedItems.ToArray();
                selectedItems = selectedItems.Where<object>(s => s is Article).ToArray();
                Article[] articles = Array.ConvertAll<object, Article>(selectedItems, s => s as Article);
                var parentGroups = articles.GroupBy(a => a.Parent);
                if(parentGroups.Count() != 1)
                {
                    MessageBox.Show("所选 Article 不属于同一 Archives !", null, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return;
                }
                var isGroups = articles.GroupBy(a => a.IsGroup);
                if (isGroups.Count() != 1  && isGroups.FirstOrDefault().FirstOrDefault().IsGroup )
                {
                    MessageBox.Show("所选 Article 含有已建立组的元素!", null, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return;
                }

                Article[] dstArticles = parentGroups.FirstOrDefault().ToArray();
                if( this.ArchivesService != null)
                {
                    this.ArchivesService.BuildGroup(articles);
                }

            }
            throw new NotImplementedException();
        }

        public SfNavigationDrawer Drawer { get; set; }

        private ICommand _addArticleCommand;
        public ICommand AddArticleCommand
        {
            get
            {
                if (_addArticleCommand == null)
                {
                    _addArticleCommand = new DelegateCommand<object>(AddArticle);
                }

                return _addArticleCommand;
            }
        }

        private void AddArticle(object obj)
        {
            if (obj is TreeGridNodeContextMenuInfo menuInfo)
            {
                object[] selectedItems = menuInfo.TreeGrid.SelectedItems.ToArray();
                Archive dstArchives = selectedItems.Where(s => s is Archive).FirstOrDefault() as Archive;
                if( Drawer != null)
                {
                    DrawerContentView drawerContentView = ContainerLocator.Container.Resolve<DrawerContentView>();
                    drawerContentView.btnOk.Click += BtnOk_Click;
                    drawerContentView.btnCansol.Click += BtnCansol_Click;
                    Drawer.DrawerContentView = drawerContentView;
                    Drawer.ToggleDrawer();
                }
            }
        }

        private void BtnCansol_Click(object sender, RoutedEventArgs e)
        {

            
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
/*
        #region Filtering
        internal FilterChanged filterChanged;

        *//*        private bool MakeStringFilter(EmployeeInfo o, string option, string condition)
                {
                    var value = o.GetType().GetProperty(option);
                    var exactValue = value == null && option == "EmployeeID" ? (o as EmployeeInfo).ID : value.GetValue(o, null);
                    exactValue = exactValue.ToString().ToLower();
                    string text = FilterText.ToLower();
                    var methods = typeof(string).GetMethods();
                    if (methods.Count() != 0)
                    {
                        var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
                        bool result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text });
                        return result1;
                    }
                    else
                        return false;
                }*//*
        private bool MakeStringFilter(Detail o, string option, string condition)
        {
            string value = o.GetType().GetProperty(option).GetValue(o, null) as string;

            return value.Contains(FilterText);
        }

        private bool MakeNumericFilter(EmployeeInfo o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value == null && option == "EmployeeID" ? (o as EmployeeInfo).ID : value.GetValue(o, null);
            double res;
            bool checkNumeric = double.TryParse(exactValue.ToString(), out res);
            if (checkNumeric)
            {
                switch (condition)
                {
                    case "Equals":
                        if (Convert.ToDouble(exactValue) == (Convert.ToDouble(FilterText)))
                            return true;
                        break;
                    case "GreaterThan":
                        if (Convert.ToDouble(exactValue) > Convert.ToDouble(FilterText))
                            return true;
                        break;
                    case "LessThan":
                        if (Convert.ToDouble(exactValue) < Convert.ToDouble(FilterText))
                            return true;
                        break;
                    case "NotEquals":
                        if (Convert.ToDouble(FilterText) != Convert.ToDouble(exactValue))
                            return true;
                        break;
                }
            }
            return false;
        }

        public bool FilerRecords(object o)
        {
            double res;
            bool isArchives = o is Archives;    
            if (o != null && string.IsNullOrEmpty(FilterText)) 
                return true;
            
            if (o != null)
            {
                Detail detail = isArchives ? (o as Archives).Detail : (o as Article).Detail;
                if (FilterOption.Equals("AllColumns"))
                {
                    return detail.Title.Contains(FilterText) ||
                        detail.SubTitle.Contains(FilterText) ||
                        detail.Measure.Contains(FilterText) ||
                        detail.Owner.Contains(FilterText) ||
                        detail.Dossier.Contains(FilterText) ||
                        detail.Year.Contains(FilterText) ||
                        detail.Copies.Contains(FilterText) ||
                        detail.Pages.Contains(FilterText) ||
                        detail.Address.Contains(FilterText);
                }
                else
                {
                    *//*if (FilterCondition == null || FilterCondition.Equals("Equals") || FilterCondition.Equals("LessThan") || FilterCondition.Equals("GreaterThan") || FilterCondition.Equals("NotEquals"))
                        FilterCondition = "Contains";*//*
                    
                    bool result = detail != null ?  MakeStringFilter(detail, FilterOption, FilterCondition) : false;
                    return result;
                }
            }
            return false;
        }

        private string _filterOption = "AllColumns";
        public string FilterOption
        {
            get { return _filterOption; }
            set
            {
                _filterOption = value.Replace(" ", "");
                if (filterChanged != null)
                    filterChanged();
                SetProperty(ref _filterOption, value);
            }
        }

        private string _filterText = string.Empty;

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                if (filterChanged != null)
                    filterChanged();
                SetProperty(ref _filterText, value);
            }
        }

        private string _filterCondition;
        
        public string FilterCondition
        {
            get { return _filterCondition; }
            set
            {
                _filterCondition = value.Replace(" ", "");
                if (filterChanged != null)
                    filterChanged();
                SetProperty(ref _filterCondition, value);
            }
        }
        #endregion*/
    }
}
