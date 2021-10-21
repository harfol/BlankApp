
using BlankApp.Service.Model;
using BlankApp.Service.Impl;

namespace BlankApp.Service
{
    public interface IArticleService
    {
        Article[] Read(string artiPath);
        bool IsArticleDirectory(string str);
        bool IsMultipleArticleDirectory(string str);
        bool WriteTxtProperty(string txtFile, string key = "DefaultKey", string value = "");
        bool WriteTxtProperties(string txtFile, Detail detail);
        Detail ReadTxtProperties(string txtFile);
        string ReadTxtProperty(string txtFile, string key = "DefaultKey");
        string GetTxtFileName(string artiPath);
        string[] GetPdfFileName(string artiPath);
        ArticleToken GetArticleToken(string artiPath);
        ArticleToken[] GetArticleTokens(string artiPath);

        string[] SplitPdf(string pdfPath);

        string GetPdfTxtPage0(string artiPath);
    }

}
