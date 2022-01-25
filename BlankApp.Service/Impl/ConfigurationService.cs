using BlankApp.Configuration;
using BlankApp.Configuration.Models;
using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace BlankApp.Service.Impl
{
    public class ConfigurationService : IConfigurationService
    {
        public Dictionary<string, BlankApp.Configuration.Models.Project> ProjectSettings => Settings.ProjectSettings;
        public Dictionary<string, string> AppSettings => Settings.AppSettings;
        public Dictionary<string, string> CorrectionSettings => Settings.CorrectionSettings;

        public object this[string str]
        { 
            get
            {
                object v = Settings.AppSettings[str] as object;
                if( v == null && Settings.ProjectSettings .ContainsKey(str))
                {
                    v = Settings.ProjectSettings[str] ;
                }
                return v;
                
            }
        }
        public ConfigurationService()
        {
            Settings.Empty();
        }


        public WidthPair[] ReadWidthPairs(string cfgPath)
        {

   
            string[] lines = File.ReadAllLines(cfgPath);
            WidthPair[] widthPairs = new WidthPair[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("="))
                {
                    string[] ks = lines[i].Split('=');

                    widthPairs[i].Numbers = Array.ConvertAll<string, int>(ks[0].Split(','), int.Parse);
                    int num = 0;
                    if (int.TryParse(lines[i].Split('=')[1], out num))
                    {
                        widthPairs[i].Width = num;
                    }
                    else
                    {
                        widthPairs[i].Width = 0;
                    }
                }
            }

            return widthPairs;
        }

    }
}
