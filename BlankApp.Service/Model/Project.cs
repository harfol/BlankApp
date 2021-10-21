
using BlankApp.Service.Model.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Model
{
    public class Project
    {
        public ProjectObject Raw { get; set; }


        public string[] ArchivesSourcePaths { get; set; }
        public ObservableCollection<Archive> Archives { get; set; }

    }
}
