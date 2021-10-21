using BlankApp.Model;
using BlankApp.ViewModel;
using BlankApp.Views;
using Microsoft.Xaml.Behaviors;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlankApp.Behaviors
{
    public class FilteringBehavior : Behavior<UserControl>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AssociatedObject.DataContext is FilteringViewModel filteringViewModel)
            {
                filteringViewModel.Drawer = AssociatedObject.drawer;
                filteringViewModel.filterChanged = OnFilterChanged;
                AssociatedObject.treeGrid.CurrentCellBeginEdit += TreeGrid_CurrentCellBeginEdit;
                AssociatedObject.treeGrid.CurrentCellEndEdit += TreeGrid_CurrentCellEndEdit;

            }


        }

        private void TreeGrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            ;
        }

        private void TreeGrid_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {
            ;
        }

        private void OnFilterChanged()
        {
            var treeGrid = this.AssociatedObject.treeGrid;
            var viewModel = this.AssociatedObject.DataContext as FilteringViewModel;
            if (treeGrid.View != null)
            {
                treeGrid.View.Filter = viewModel.FilerRecords;
                treeGrid.View.RefreshFilter();
            }
        }

    }
}
