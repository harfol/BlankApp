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
        private static string DllFile = "BlankApp.Configuration.dll";
        private static string DllFileName = Assembly.GetExecutingAssembly().CodeBase + ".config";
        
        private static string ExeFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private static System.Configuration.Configuration _config;


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

        public static void Empty()
        {

        }

        public static string Get(string section, string key)
        {
            return  (ConfigurationManager.GetSection(section) as NameValueCollection)?.Get(key);
        }

        public static object GetSection(string section)
        {
            object o = null;
            return o;
        }


        private static void Load()
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(DllFile);

            AppSettingsSection appSettingsSection = configuration.GetSection("correctionSettings") as AppSettingsSection;
            ProjectSettingsSection projectSettingsSection = configuration.GetSection("projectSettings") as ProjectSettingsSection;
            foreach (string item in appSettingsSection.Settings.AllKeys)
            {
                AppSettings.Add(item, appSettingsSection.Settings[item].Value);
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

         static Settings()
        {


            _projectSettings = new Dictionary<string, Project>();
            _appSettings = new Dictionary<string, string>();
            Load();
            //XmlDocument doc = new XmlDocument();
            //// Project
            //_projects = new Dictionary<string, Project>();
            //doc = new XmlDocument();
            //doc.Load(DllFileName);
            //XmlNodeList nodes = doc.GetElementsByTagName("project");
            //for (int i = 0; i < nodes.Count; i++)
            //{
            //    Project project = new Project();
            //    project.Name = nodes[i].Attributes["name"].Value;
            //    project.Describe = nodes[i].Attributes["describe"].Value;
            //    project.Remote = nodes[i].Attributes["remote"].Value;

            //    _projects.Add(project.Name, project);
            //}

            //_appSettings = new Dictionary<string, string>();
            //nodes = doc.GetElementsByTagName("add");
            //for (int i = 0; i < nodes.Count; i++)
            //{
             
            //    string key = nodes[i].Attributes["key"].Value;
            //    string value = nodes[i].Attributes["value"].Value;
            //    _appSettings.Add(key, value);
            //}
        }
    }
}
