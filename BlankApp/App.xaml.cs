using BlankApp.Services;
using BlankApp.Services.Impl;

using BlankApp.ViewModel;
using BlankApp.Views;
using Prism.Ioc;
using System.Collections.Generic;
using System.Windows;
using BlankApp.Behaviors;
using BlankApp.Service.Model.Object;
using BlankApp.Service.Impl;

namespace BlankApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDI4NjY2QDMxMzkyZTMxMmUzMEZiM1h6bkQ2Y0VIRUpHdGtLMUFXYUxKMXFYZHJWdWM4Ukh1aDVlRmoxc009");
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
          
            containerRegistry.RegisterInstance<Dictionary<string, object>>(new Dictionary<string, object>(), "ArchivesDetails");
            containerRegistry.RegisterInstance<ProjectObject>(new ProjectObject(), "CurrentProject");
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();
            containerRegistry.RegisterSingleton<IMaskService, MaskService>();
            containerRegistry.RegisterSingleton<IArticleService, RawArticleService>();
            containerRegistry.RegisterSingleton<IArchiveService, ArchivesService>();
            containerRegistry.RegisterSingleton<IProjectService, ProjectService>();

            containerRegistry.Register<NavationItemClickedAction>();
      
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<Hierarchical, HierarchicalViewModel>();
            containerRegistry.RegisterForNavigation<DrawerContentView, DrawerContentViewModel>();
            containerRegistry.RegisterForNavigation<Filtering, FilteringViewModel>();
        }
    }
}
