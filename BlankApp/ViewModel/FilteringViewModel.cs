using BlankApp.Service;
using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using Prism.Commands;
using Prism.Mvvm;
using Syncfusion.UI.Xaml.NavigationDrawer;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlankApp.ViewModel
{
    public class FilteringViewModel : BindableBase
    {
        private readonly IArchiveService _archivesService;
        private IMaskService _maskService;
        private IConfigurationService _configurationService;

        #region Propery
        private ObservableCollection<Archive> _archivesDetails;
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


        private string[] _autoCompleteSource;
        public string[] AutoCompleteSource
        {
            get { return _autoCompleteSource; }
            set { SetProperty(ref _autoCompleteSource, value); }
        }

        #endregion
        public string CurrentProject { get; set; }

        public FilteringViewModel(IArchiveService archivesService, IMaskService maskService, IConfigurationService configurationService)
        {
            this._archivesService = archivesService;
            this._maskService = maskService;
            this._configurationService = configurationService;

            TitleObject[] titles = this._configurationService["Titles"] as TitleObject[];
            AutoCompleteSource = Array.ConvertAll<TitleObject, string>(titles, t => t.Name);
            List<string> fields = Array.ConvertAll<PropertyInfo, string>(typeof(Detail).GetProperties(), p => p.Name).ToList();
            fields.Insert(0, "AllColumns");
            fields.RemoveAt(fields.Count - 1);
            SearchFields = fields.ToArray();

            TextBlock textBlock = new TextBlock();
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
            if (obj is TreeGridNodeContextMenuInfo menuInfo)
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
                if (parentGroups.Count() != 1)
                {
                    MessageBox.Show("所选 Article 不属于同一 Archives !", null, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return;
                }
                var isGroups = articles.GroupBy(a => a.IsGroup);
                if (isGroups.Count() != 1 && isGroups.FirstOrDefault().FirstOrDefault().IsGroup)
                {
                    MessageBox.Show("所选 Article 含有已建立组的元素!", null, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return;
                }

                Article[] dstArticles = parentGroups.FirstOrDefault().ToArray();
                if (this._archivesService != null)
                {
                    this._archivesService.BuildGroup(articles);
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
                /*                object selectedItem = menuInfo.TreeGrid.SelectedItem;
                                Archives dstArchives = selectedItem is Archives ? selectedItem as Archives 
                                    : (selectedItem as Article)?.Parent;
                                if (Drawer != null && dstArchives != null)
                                {
                                    DrawerContentView drawerContentView = ContainerLocator.Container.Resolve<DrawerContentView>();
                                    (drawerContentView.DataContext as DrawerContentViewModel).CurrentArchives = dstArchives;

                                    drawerContentView.btnOk.Click += BtnOk_Click;
                                    drawerContentView.btnCansol.Click += BtnCansol_Click;
                                    Drawer.DrawerContentView = drawerContentView;
                                    Drawer.ToggleDrawer();
                                }*/

                Archive archives = menuInfo.TreeGrid.SelectedItem as Archive;
                archives.Nodes.Add(new Article(archives.Nodes.Count() + 1, "", false, ArticleTypes.Normal) 
                {
                    Detail = new Detail()
                });
            }
        }

        private void BtnCansol_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if( button.DataContext  is DrawerContentViewModel drawervm)
            {
                string project =  drawervm.Project.Text;
                string position = drawervm.Position.Text;
                string title = drawervm.Title.Text;
                string addition = drawervm.Addition.Text;
                Archive archives = drawervm.CurrentArchives;

                int index = archives.Nodes.Count + 1;
                string mask = this._maskService.Simplify(title) + (!string.IsNullOrEmpty(addition) ?  $"{{{addition}}}" : "");
                //Article article = new Article(index.ToString("{0:2D}"), mask, false, ArticleTypes.Normal);
                Archive src = this.ArchivesDetails.Where(a => a.Equals(archives)).FirstOrDefault();
                //src.Nodes.Add(article);
            }
 
        }

        #endregion

        #region Filtering
        internal delegate void FilterChanged();
        internal FilterChanged filterChanged;

        /*        private bool MakeStringFilter(EmployeeInfo o, string option, string condition)
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
                }*/
        private bool MakeStringFilter(Detail o, string option, string condition)
        {
            string value = o.GetType().GetProperty(option).GetValue(o, null) as string;

            return value.Contains(FilterText);
        }

        public bool FilerRecords(object o)
        {
            double res;
            bool isArchives = o is Archive;
            if (o != null && string.IsNullOrEmpty(FilterText))
                return true;

            if (o != null)
            {
                Detail detail = isArchives ? (o as Archive).Detail : (o as Article).Detail;
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
                    /*if (FilterCondition == null || FilterCondition.Equals("Equals") || FilterCondition.Equals("LessThan") || FilterCondition.Equals("GreaterThan") || FilterCondition.Equals("NotEquals"))
                        FilterCondition = "Contains";*/

                    bool result = detail != null ? MakeStringFilter(detail, FilterOption, FilterCondition) : false;
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
        #endregion
    }
}
