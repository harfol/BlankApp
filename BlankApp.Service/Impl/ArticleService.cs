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
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Copies { get; set; }
    }

    public class ArticleService : ArticleServiceBase
    {
        private readonly string _txtFileName = "信息.txt";

        public override bool IsArticleDirectory(string str)
        {
            string first = Path.GetFileName(str);
            first = first.Trim().Split('-').First();
            Regex regex = new Regex(@"^\d{2}\.[\u4e00-\u9fa5]+");
            return regex.Match(first ?? "").Success;
        }
        public override Article[] Read(string artiPath)
        {
            if( IsArticleDirectory(artiPath))
            {
                string path = Path.GetFileName(artiPath);
                Article[] articles = ExtractArticle(path);

                string txt = Path.Combine(artiPath, _txtFileName);
                if (File.Exists(txt))
                {
                    Detail detail = ReadTxtProperties(txt);
                    string[] ss = detail.Title.Split('-');
                    for (int i = 0; i < ss.Length; i++)
                    {
                        articles[i].Detail = detail.Clone();
                    }

                }
                return articles;
            }
            return null;
        }
        public override string GetTxtFileName(string artiPath)
        {
            Info[] infos = ExtractInfo(artiPath);
            string name = "";
            foreach (Info info in infos)
            {
                name += info.Name + "-";
            }
            name.Remove(name.Length - 1);
            return Path.Combine(name, ".txt");
        }
        private Info[] ExtractInfo(string artiPath)
        {
            string file = Path.GetFileName(artiPath);
            string[] split = file.Split('-');
            Info[] ret = new Info[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                Info info = new Info();
                string name = split[i];
                // 获取份数并去掉
                string copies = "";
                if(split.Length > 1)
                {
                    copies = 1.ToString();
                }
                else
                {
                    while (name[name.Length - 1] >= '0' && name[name.Length - 1] <= '9')
                    {
                        copies = copies.Insert(0, name.Last().ToString());
                        name = name.Substring(0, name.Length - 1);
                    }
                }
                info.Caption = copies;
                // id
                if (name.Contains('.'))
                {
                    info.Id = name = name.Substring(3);
                }
                // caption
                int l = name.IndexOf('{');
                int r = name.LastIndexOf('}');
                if (l >= 0 && r >= 1)
                {
                    string[] names = name.Split('{', '}');
                    string namel = names[0];
                    string namer = names[2];
                    string caption = names[1];
                    info.Name = namel + namer;
                    info.Caption = caption;
                }
                
                ret[i] = info;
            }
            return ret;
        }
        private Article[] ExtractArticle(string artiPath)
        {

               Info[] infos = ExtractInfo(artiPath);
            Article[] ret = new Article[infos.Length];
            for (int i = 0; i < infos.Length; i++)
            {
                ret[i] = new Article();
                ret[i].Id = int.Parse(infos[i].Id);
                ret[i].Mask = infos[i].Name;
                ret[i].Caption = infos[i].Caption;
                ret[i].IsGroup = false;

            }
            //isgroup
            for (int i = 0; i < ret.Length && ret.Length > 1; i++)
            {
                ret[i].IsGroup = true;
            }
            return ret;
        }

        public override bool IsMultipleArticleDirectory(string str)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetPdfFileName(string artiPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
