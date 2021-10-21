using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;

namespace BlankApp.Service.Impl
{
    public class ConfigurationService : IConfigurationService
    {
        private string[] _positions;
        private TitleObject[] _titles;
        private ProjectObject[] _projects;
        private string _cfgFilePath = "Config/dirinfo.json";

        public object this[string str]
        {
            get
            {
                if (str.Equals("Titles"))
                {
                    return this._titles;
                }
                else if (str.Equals("Projects"))
                {
                    return this._projects;
                }
                else if (str.Equals("Positions"))
                {
                    return this._positions;
                }
                return null;
            }
        }


        public ConfigurationService()
        {
            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            _cfgFilePath =  Path.Combine(path, "Config", "dirinfo.json");
            Reflash();
        }

        public void Reflash()
        {
            using (System.IO.StreamReader file = System.IO.File.OpenText(_cfgFilePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
          
                    this._projects = o["Projects"].ToObject<ProjectObject[]>();
                    this._positions = o["Positions"].ToObject<string[]>();
                    this._titles = o["Titles"].ToObject<TitleObject[]>();
                }
            }
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
