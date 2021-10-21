using BlankApp.Service.Model;

namespace BlankApp.Service
{
    public interface IConfigurationService
    {
        object this[string str] { get; }
        WidthPair[] ReadWidthPairs(string cfgPath);
    }
}
