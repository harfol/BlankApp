using BlankApp.Service.Model;
using iTextSharp.text;
using iTextSharp.text.io;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlankApp.Service.Impl
{
    public abstract class ArticleServiceBase: IArticleService
    {

        public abstract Article[] Read(string artiPath);
        public abstract bool IsArticleDirectory(string str);
        public abstract string GetTxtFileName(string artiPath);
        public abstract string[] GetPdfFileName(string artiPath);

        public abstract bool IsMultipleArticleDirectory(string str);

        public ArticleToken GetArticleToken(string artiPath)
        {
            string name = System.IO.Path.GetFileName(artiPath);
            // 获取 份数， 修改名称
            string copies = "";
            while (name.Last() >= '0' && name.Last() <= '9')
            {
                copies = copies.Insert(0, name.Last().ToString());
                name = name.Remove(name.Length - 1);
            }
            // 获取 id
            string id = "";
            if (name.Contains('.') || Regex.IsMatch(name, @"^\d{2}?"))
            {
                id = name.Substring(0, 2);
                name = name.Substring(2);
            }
            
            //获取说明
            string caption = "";
            int l = name.IndexOf('{');
            int r = name.LastIndexOf('}');
            if (l >= 0 && r >= 1)
            {
                string[] names = name.Split('{', '}');
                string namel = names[0];
                string namer = names[2];
                caption = names[1];
                name = namel + namer;
            }
            return new ArticleToken()
            {
                Id = id,
                Name = name,
                Caption = caption,
                Copies = string.IsNullOrEmpty(copies) ? 1.ToString() : copies
            };
        }
        public ArticleToken[] GetArticleTokens(string artiPath)
        {
            string file = System.IO.Path.GetFileName(artiPath);
            string[] split = file.Split('-');
            ArticleToken[] ret = new ArticleToken[split.Length];

            for (int i = 0; i < split.Length; i++)
            {
                ret[i] = GetArticleToken(split[i]);
            }
            
            return ret;
        }
        public Detail ReadTxtProperties(string txtFile)
        {

            Detail map = new Detail();
            string[] lines = File.ReadAllLines(txtFile);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("："))
                {
                    string[] str = lines[i].Split('：');
                    map[str[0]] = str[1];
                }
            }

            return map;
        }
        public string ReadTxtProperty(string txtFile, string key = "DefaultKey")
        {
            using (StreamReader sr = File.OpenText(txtFile))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string c;
                    if (s.Trim().Contains(key) && (c = s.Trim().Replace($"{key}：", "")) != null)
                    {
                        return c;
                    }
                }
            }
            return null;
        }
        public bool WriteTxtProperties(string txtFile, Detail detail)
        {
            if (txtFile.EndsWith(".txt"))
            {
                if (!File.Exists(txtFile))
                {
                    string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    File.Copy(System.IO.Path.Combine(path, "Template", "信息.txt"), txtFile);

                }

                string[] lines = File.ReadAllLines(txtFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("："))
                    {
                        string[] str = lines[i].Split('：');
                        lines[i] = str[0] + "：" + detail[str[0]];
                    }
                }

                File.WriteAllLines(txtFile, lines);
                return true;
            }
            return false;
        }
        public bool WriteTxtProperty(string txtFile, string key = "DefaultKey", string value = "")
        {
            string[] lines = File.ReadAllLines(txtFile);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(key))
                {
                    lines[i] = $"{key}：{value}";
                    File.WriteAllLines(txtFile, lines);
                    return true;
                }
            }
            return false;
        }


        public string[] SplitPdf(string pdfPath)
        {
            List<string> pdfs = new List<string>();
            using (var reader = new PdfReader(pdfPath))
            {
                string dir = System.IO.Path.GetDirectoryName(pdfPath);
                // 注意起始页是从1开始的
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    using (var sourceDocument = new Document(reader.GetPageSizeWithRotation(i)))
                    {
                        string pdf = System.IO.Path.Combine(dir, $"{i}.pdf");
                        var pdfCopyProvider = new PdfCopy(sourceDocument, new FileStream(pdf, FileMode.Create));
                        sourceDocument.Open();
                        var importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                        pdfs.Add(pdf);
                    }
                }
            }
            return pdfs.ToArray();
        }
        public string GetPdfTxtPage0(string pdfPath)
        {
            string s = null;
            StreamUtil.AddToResourceSearch(System.Reflection.Assembly.LoadFile(@"C:\Users\admin\source\repos\BlankApp\bin\Debug\iTextAsian.dll"));
            StreamUtil.AddToResourceSearch(System.Reflection.Assembly.LoadFile(@"C:\Users\admin\source\repos\BlankApp\bin\Debug\iTextAsianCmaps.dll"));
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                s = PdfTextExtractor.GetTextFromPage(reader, 1, new LocationTextExtractionStrategy()); ;
            }
            return s;
        }


    }
}
