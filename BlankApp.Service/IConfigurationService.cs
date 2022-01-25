using BlankApp.Service.Model;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlankApp.Service
{
    public interface IConfigurationService
    {
        Dictionary<string, BlankApp.Configuration.Models.Project> ProjectSettings { get; }
        Dictionary<string, string>  AppSettings { get; }
        Dictionary<string, string> CorrectionSettings { get; }
        object this[string str] { get; }
        WidthPair[] ReadWidthPairs(string cfgPath);
    }
}
