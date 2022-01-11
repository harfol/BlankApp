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
        private static string FileName = Assembly.GetExecutingAssembly().CodeBase + ".config"; //AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private static Dictionary<string, Project> _projects;
        public static Dictionary<string, Project> Projects
        {
            get => _projects;
        }


        private static Dictionary<string, string> _appSettings;

        public static Dictionary<string, string> AppSettings
        {
            get => _appSettings;
        }

        public static void Empty()
        {

        }

        static Settings()
        {
            // Project
            _projects = new Dictionary<string, Project>();
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);
            XmlNodeList nodes = doc.GetElementsByTagName("project");
            for (int i = 0; i < nodes.Count; i++)
            {
                Project project = new Project();
                project.Name = nodes[i].Attributes["name"].Value;
                project.Describe = nodes[i].Attributes["describe"].Value;
                project.Remote = nodes[i].Attributes["remote"].Value;

                _projects.Add(project.Name, project);
            }

            _appSettings = new Dictionary<string, string>();
            nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
             
                string key = nodes[i].Attributes["key"].Value;
                string value = nodes[i].Attributes["value"].Value;
                _appSettings.Add(key, value);
            }
        }
    }
}
