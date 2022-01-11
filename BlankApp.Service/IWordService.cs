using BlankApp.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service
{
    public interface IWordService
    {
        void CreateNominate();

        CoverToken BuildCoverToken(int projectNumber, int number, string projectName, string title, string subTitle, string owner, string measure);
        void BuildCover(string coverPath, CoverToken token);
        void PrintCover(string coverPath);

        TitleToken BuildTitleToken(int width, int porjectNumber, int[] id, string projectName, string title);
        void BuildTitle(string tmpPath, TitleToken[] tokens, string demoName);
        void BuildCatalog(string catalogPath, string dossier, Catalog[] catalogs);
        void BuildCatalog(string catalogPath, string dossier, Catalog[] catalogs, Action<string> action);
        Catalog[] ReadCatalog(string catalogPath);


        



    }
}
