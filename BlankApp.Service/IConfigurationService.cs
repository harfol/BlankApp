using BlankApp.Service.Model;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlankApp.Service
{
    public interface IConfigurationService
    {
        Dictionary<string, BlankApp.Configuration.Models.Project> ProjectSettings { get; }
        Dictionary<string, string>  AppSettings { get; }
         NameValueCollection Correction { get; }
        string Get(string section, string key);

        NameValueCollection GetSection(string section);
        object this[string str] { get; }
        WidthPair[] ReadWidthPairs(string cfgPath);
    }
}
