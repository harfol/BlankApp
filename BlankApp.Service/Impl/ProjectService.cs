using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlankApp.Service.Impl
{
    public class ProjectService : IProjectService
    {
        private readonly IConfigurationService _configurationService;
        private Dictionary<string, Project> _projectCollection;


        public ProjectService(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this._projectCollection = new Dictionary<string, Project>();
            LoadProject();
        }

        private void LoadProject()
        {
            ProjectObject[] projectObjects = this._configurationService["Projects"] as ProjectObject[];
            foreach (ProjectObject obj in projectObjects)
            {
                Project project = new Project();
                project.Raw = obj;
                this._projectCollection.Add(obj.Name ?? "default", project);
            }
        }
        private bool IsArchiveDirectory(string str)
        {
            string first = Path.GetFileNameWithoutExtension(str);
            first = first.Trim().Split('-').First();
            Regex regex = new Regex(@"^\d{4}?");
            return regex.Match(first ?? "").Success;
        }

        #region 档案

        public bool Contains(string name)
        {
            return this._projectCollection.ContainsKey(name);
        }
        public bool HasArchives(string name)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                return this._projectCollection[name].Archives != null;

            }
            return false;
        }
        public ObservableCollection<Archive> GetArchives(string name)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                return this._projectCollection[name].Archives;

            }
            return null;
        }
        public bool SetArchives(string name, ObservableCollection<Archive> archives)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                this._projectCollection[name].Archives = archives;
                return this._projectCollection[name].Archives != null; 

            }
            return false;
        }


        public void RefreshArchivalSourcePaths(string name)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                string sourcePath = this._projectCollection[name].Raw.Path;
                string[] array = Directory.GetDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly).Where(IsArchiveDirectory).ToArray();
                SetArchivalSourcePaths(name, array);
            }
        }
        public string[] GetArchivalSourcePaths(string name)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                return this._projectCollection[name].ArchivesSourcePaths;
               
            }
            return null;
        }
        public bool SetArchivalSourcePaths(string name, string[] paths)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                this._projectCollection[name].ArchivesSourcePaths = paths;
                return this._projectCollection[name].Archives != null;

            }
            return false;
        }

        #endregion

        #region 项目
        public string[] GetProjectNames()
        {
            return this._projectCollection.Keys.ToArray();
        }
        public string GetProjectSourcePath(string name)
        {
            if (this._projectCollection.ContainsKey(name))
            {
                return this._projectCollection[name].Raw?.Path ?? "";
            }
            return null;
        }
        public string[] GetProjectSourcePaths()
        {

            return Array.ConvertAll<Project, string>(this._projectCollection.Values.ToArray(), v => v.Raw?.Path);
        }
        #endregion



    }
}
