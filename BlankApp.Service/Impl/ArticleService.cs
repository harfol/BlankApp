using BlankApp.Service.Impl;
using BlankApp.Service.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlankApp.Service.Impl
{
    public class Info
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Copies { get; set; }
    }

    public class ArticleService : ArticleServiceBase
    {
        private readonly string _txtFileName = "信息.txt";

        public override bool IsArticleDirectory(string str)
        {
            string name = Path.GetFileName(str);
 
            Regex regex = new Regex(@"^\d{2}[\u4e00-\u9fa5]+");
            return regex.Match(name ?? "").Success;
        }
        public override Article[] Read(string artiPath)
        {
            if( IsArticleDirectory(artiPath))
            {
                string path = Path.GetFileName(artiPath);
                ArticleToken articleToken = GetArticleToken(path);
                Article article = new Article();
                article.Mask = articleToken.Name;
                article.Id = int.Parse(articleToken.Id);

                string txt = Path.Combine(artiPath, articleToken.Name + ".txt");
                if (File.Exists(txt))
                {
                    article.Detail = ReadTxtProperties(txt);
                    article.TxtPath = txt;
                }

                string pdf = Path.Combine(artiPath, articleToken.Name + ".pdf");
                if (File.Exists(pdf))
                {
                    article.PdfPaths = new string[1] { pdf } ;
                }
                return new Article[1] { article };
            }
            return null;
        }
        private Article ExtractArticleInfo(string artiPath)
        {
            string name = Path.GetFileName(artiPath);
            string copies = "";
            string id = name.Substring(0, 2);
            name = name.Remove(0, 2);
            while (name[name.Length - 1] >= '0' && name[name.Length - 1] <= '9')
            {
                copies = copies.Insert(0, name.Last().ToString());
                name = name.Remove(name.Length - 1);
            }


            Article article = new Article();
            article.Id = int.Parse(id);
            article.Mask = name;
            article.Caption = copies;

            return article;
        }

        public override bool IsMultipleArticleDirectory(string str)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetPdfFileName(string artiPath)
        {
            throw new System.NotImplementedException();
        }

        public override string GetTxtFileName(string artiPath)
        {
            throw new System.NotImplementedException();
        }


    }
}
