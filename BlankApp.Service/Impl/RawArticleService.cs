using BlankApp.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BlankApp.Service;

namespace BlankApp.Service.Impl
{
    public class RawArticleService : ArticleServiceBase
    {
        private IMaskService _ms;

        public RawArticleService(IMaskService ms)
        {
            this._ms = ms;
        }
        public override string GetTxtFileName(string artiPath)
        {
            if( IsArticleDirectory(artiPath))
            {
                string[] txts = Directory.GetFiles(artiPath, "*.txt", SearchOption.TopDirectoryOnly);
                return (txts != null && txts.Length == 1) ? txts[0] : null;
            }
            return null;
        }
        public override string[] GetPdfFileName(string artiPath)
        {
            if (IsArticleDirectory(artiPath))
            {
                return  Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly);
            }
            return null;
        }

        public override bool IsArticleDirectory(string str)
        {
            return true;
        }
        public override bool IsMultipleArticleDirectory(string str)
        {
            return str.Contains("-");
        }

        private Article[] ExtractGroupArticle(Detail detail)
        {
            List<Article> articles = new List<Article>();
            foreach (string key in detail.Title.Split('-'))
            {
                Article article = new Article();
                article.Type = ArticleTypes.Normal;
                article.Mask = _ms.Simplify(key) ?? key;
                article.IsGroup = true;
                article.Detail = detail.Clone();
                article.Detail.Title = key;
                articles.Add(article);
            }
            return articles.ToArray();
        }
        private Article ExtractNormalArticle(Detail detail)
        {
            Article article = new Article();
            article.Type = ArticleTypes.Normal;
            article.Mask = _ms.Simplify(detail.Title) ?? detail.Title;
            article.Detail = detail.Clone();
            return article;
        }

        public override Article[] Read(string artiPath)
        {
            string txt = GetTxtFileName(artiPath);
            string[] pdfs = GetPdfFileName(artiPath);
            List<Article> articles = new List<Article>();
            if( !string.IsNullOrEmpty(txt))
            {
                Detail detail = ReadTxtProperties(txt);
                if( detail.Title.Contains('-'))
                {
                    Article[] artis = ExtractGroupArticle(detail);
                    Array.ForEach(artis, a => 
                    {
                        a.TxtPath = txt;
                        a.PdfPaths = pdfs;
                        a.ArticlePath = artiPath;
                    });
                    articles.AddRange(artis);
                }
                else
                {
                    Article arti = ExtractNormalArticle(detail);
                    arti.TxtPath = txt;
                    arti.PdfPaths = pdfs;
                    arti.ArticlePath = artiPath;
                    articles.Add(arti);
                }
            }
            return articles.ToArray();
        }
    }
}
