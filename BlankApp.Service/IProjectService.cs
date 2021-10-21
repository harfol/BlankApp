using BlankApp.Service.Model;
using System.Collections.ObjectModel;

namespace BlankApp.Service
{
    public interface IProjectService
    {

        bool Contains(string name);
        bool HasArchives(string name);
        ObservableCollection<Archive> GetArchives(string name);
        bool SetArchives(string name, ObservableCollection<Archive> archives);


        void RefreshArchivalSourcePaths(string name);
        string[] GetArchivalSourcePaths(string name);
        bool SetArchivalSourcePaths(string name, string[] paths);


        string[] GetProjectNames();
        string GetProjectSourcePath(string name);
        string[] GetProjectSourcePaths();
    }
}
