using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace BlankApp.Model
{
    public class HierarchicalMenu
    {
        public string Header { get; set; }
        public Path Icon { get; set; }
        public ObservableCollection<HierarchicalMenu> SubItems { get; set; }
    }
}
