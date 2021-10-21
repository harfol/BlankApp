using BlankApp.Model;
using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using BlankApp.Services;
using Prism.Ioc;
using Prism.Mvvm;
using System;

namespace BlankApp.ViewModel
{
    public class DrawerContentViewModel : BindableBase
    {
        private IContainerRegistry _containerExtension;
        private IContainerProvider _containerProvider;
        private readonly IConfigurationService _configurationService;


        public string CurrentProject { get; set; }

        private ArchivesAssignField _project;
        public ArchivesAssignField Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }


        private ArchivesAssignField _position;
        public ArchivesAssignField Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private ArchivesAssignField _title;
        public ArchivesAssignField Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ArchivesAssignField _addition;
        public ArchivesAssignField Addition
        {
            get { return _addition; }
            set { SetProperty(ref _addition, value); }
        }

        public Archive CurrentArchives { get; set; }

        public DrawerContentViewModel(IConfigurationService configurationService, IContainerProvider containerProvider)
        {

            this._containerProvider = containerProvider;
            this._configurationService = configurationService;

            string name = this._containerProvider.Resolve<ProjectObject>("CurrentProject").Name ;

            ProjectObject[] projects = this._configurationService["Projects"] as ProjectObject[];
            string[] positions = this._configurationService["Positions"] as string[];
            TitleObject[] titles = this._configurationService["Titles"] as TitleObject[];


            Project = new ArchivesAssignField { Label = "项目", Text = name, AutoCompleteSource = Array.ConvertAll<ProjectObject, string>(projects, p => p.Name) };
            Position = new ArchivesAssignField { Label = "村", Text = "", AutoCompleteSource = positions };
            Title = new ArchivesAssignField { Label = "标题", Text = "", AutoCompleteSource = Array.ConvertAll<TitleObject, string>(titles, t => t.Name) };
            Addition = new ArchivesAssignField { Label = "说明", Text = "", AutoCompleteSource = new string[] { "" } };
     }


    }
}
