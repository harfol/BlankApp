using BlankApp.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlankApp.Configuration
{
    public static  class Settings
    {
        private static string DllFile = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"BlankApp.Configuration.dll");

        private static Dictionary<string, Project> _projectSettings;
        public static Dictionary<string, Project> ProjectSettings
        {
            get => _projectSettings;
        }



        private static Dictionary<string, string> _appSettings;

        public static Dictionary<string, string> AppSettings
        {
            get => _appSettings;
        }

        private static Dictionary<string, string> _correctionSettings;

        public static Dictionary<string, string> CorrectionSettings
        {
            get => _correctionSettings;
        }

        public static void Empty()
        {

        }

        private static void Load()
        {

        }

         static Settings()
        {


            _projectSettings = new Dictionary<string, Project>();
            _appSettings = new Dictionary<string, string>();
            _correctionSettings = new Dictionary<string, string>();
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(DllFile);

            AppSettingsSection correctionSettingsSection = configuration.GetSection("correctionSettings") as AppSettingsSection;
            ProjectSettingsSection projectSettingsSection = configuration.GetSection("projectSettings") as ProjectSettingsSection;


            foreach (string item in configuration.AppSettings.Settings.AllKeys)
            {
                AppSettings.Add(item, configuration.AppSettings.Settings[item].Value);
            }
            foreach (string item in correctionSettingsSection.Settings.AllKeys)
            {
                CorrectionSettings.Add(item, correctionSettingsSection.Settings[item].Value);
            }
            foreach (ProjectElement item in projectSettingsSection.ProjectCollection)
            {
                Project project = new Project()
                {
                    Name = item.Name,
                    Describe = item.Describe,
                    Remote = item.Remote
                };
                ProjectSettings.Add(item.Name, project);
            }
        }
    }
}
