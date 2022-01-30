using BlankApp.Service;
using BlankApp.Service.Extensions;
using BlankApp.Service.Model;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BlankApp.Cli
{
    public class Msg
    {
        public int Id;
        public int Scan;
        public int Sum;
    }
    public class Util
    {
        private readonly IArticleService _articleService;
        private readonly IConfigurationService _configurationService;
        private readonly IMaskService _maskService;
        private readonly IWordService _wordService;
        private readonly IArchiveService _archiveService;
        private readonly IReportService _reportService;
        private bool _isDispose = false;

        public Util(IArticleService articleService, IConfigurationService configurationService, 
            IMaskService maskService, IWordService wordService, 
            IArchiveService archiveService, IReportService reportService)
        {
            this._articleService = articleService;
            this._configurationService = configurationService;
            this._maskService = maskService;
            this._wordService = wordService;
            this._archiveService = archiveService;
            this._reportService = reportService;
        }


        #region private
        private string[] GetMaskArchivePaths(string proPath, string mask)
        {
            Console.Write("获取目录...");
            string[] archPaths = Directory.GetDirectories(proPath, "*", SearchOption.TopDirectoryOnly).ToArray();
            Console.Write("\b\b\b\b\b\b\b\b\b\b\b过滤目录...");
            archPaths = archPaths.Where(a =>   _archiveService.IsArchiveDirectory(Path.GetFileName(a))).ToArray();
            if (!mask.Equals("*"))
            {
                int[] nums = _maskService.ComplexNumbers(mask);
                archPaths = archPaths.Where(a => nums.Contains(int.Parse(Path.GetFileName(a).Substring(0, 4)))).ToArray();
            }
            archPaths = archPaths.Where(a => Directory.GetDirectories(a).Length > 0).ToArray();
            Console.Write("\b\b\b\b\b\b\b\b\b\b\b");
            return archPaths;
        }

        private string GetTargetPath(string simplePath)
        {
            return Path.GetFullPath(simplePath);

        }

        private void Copy2Tmp(string src, string tmpPath)
        {
            string archPath = Path.GetDirectoryName(src);
            string num = Path.GetFileName(archPath).Substring(0, 4);
            Console.WriteLine($"拷贝 {num} {Path.GetFileName(src)} ...");
            File.Copy(src, Path.Combine(tmpPath, $"{num}-{Path.GetFileName(src)}"), true);
        }


        private bool Yes(string str)
        {
            Console.Write(str+ " （Y/N）：");
            string k = Console.ReadLine();
            return k.Equals("Y") || k.Equals("y");
            
        }

        private void ArchvieTitleContainsAgreement(string archPath, Action<Detail> func)
        {
            Console.Write(Path.GetFileName(archPath).Substring(0, 4));
            Archive archive = _archiveService.Read(archPath);
            Article[] articles = archive.Nodes.Where(a => a.Detail.Title.Contains("协议") || a.Detail.Title.Contains("合同")).ToArray();
            Console.Write("\b\b\b\b");
            bool flag = false;
            foreach (var a in articles.Where(a => !string.IsNullOrWhiteSpace(a.Detail.Dossier)))
            {
                if (!flag)
                {
                    Console.Write(Path.GetFileName(archPath).Substring(0, 4));
                    flag = true;
                }
                else
                {
                    Console.Write("    ");
                }
                func(a.Detail);
            }
        }

        #endregion

        #region txt文件操作

        [Status(StatuTypes.Txt, CompleteTypes.SubFunc)]
        public void 根据文件夹名称创建独立TXT文件(string artiPath, string measure, string owner, string number, string year)
        {
            artiPath = GetTargetPath(artiPath);

            // 名称 份数
            string copies, name = "";
            string rawName = Path.GetFileName(artiPath);
            if (rawName.Contains('-'))
            {
                ArticleToken[] tokens = _articleService.GetArticleTokens(artiPath);
                copies = tokens[0].Copies;
                foreach (ArticleToken token in tokens)
                {
                    name += token.Name + '-';
                }
                name = name.Remove(name.Length - 1);
            }
            else
            {
                ArticleToken token = _articleService.GetArticleToken(artiPath);
                copies = token.Copies;
                name = token.Name;
            }

            //txt路径
            string txtPath = Path.Combine(artiPath, name + ".txt");

            //pdf页数
            string[] pdfs = Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly);
            string pdf = pdfs.Length > 0 ? pdfs[0] : null;
            int page = 0;
            if ( !string.IsNullOrEmpty(pdf) )
            {


                //读pdf页数
                using (PdfReader pdfReader = new PdfReader(pdf))
                {
                    page = pdfReader.NumberOfPages;
                }
            }
            Console.WriteLine($"路径:{txtPath}");
            Console.WriteLine($"名称:{Path.GetFileNameWithoutExtension(txtPath)}");
            Console.WriteLine($"权属人:{owner}");
            Console.WriteLine($"测量:{measure}");
            Console.WriteLine($"页数:{page}");
            Console.WriteLine($"份数:{copies}");
            Console.WriteLine($"案卷号:{number}");
            Console.WriteLine();


            Detail detail = new Detail();
            detail.Title = Path.GetFileNameWithoutExtension(txtPath);
            detail.Owner = owner;
            detail.Number = number;
            detail.Copies = copies;
            detail.Measure = measure;
            detail.Year = year;
            detail.Pages = page > 0 ? page.ToString() : "";
            _articleService.WriteTxtProperties(txtPath, detail);
        }

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 根据文件夹名称创建TXT文件(string archPath, string measure, string owner, string number, string year)
        {
            archPath = GetTargetPath(archPath);

            if( _archiveService.IsArchiveDirectory(archPath))
            {
                string[] artiPaths = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly);
                foreach (string arti in artiPaths)
                {
                    根据文件夹名称创建独立TXT文件(arti, measure, owner, number, year);
                }
            }
        }

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 删除TXT文件(string archPath)
        {
            archPath = GetTargetPath(archPath);
            if( _archiveService.IsArchiveDirectory(archPath))
            {
                string[] files = Directory.GetFiles(archPath, "*.txt", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Console.WriteLine("删除TXT成功");
            } 
            
        }

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 读取字段(string archPath, string key)
        {
            archPath = GetTargetPath(archPath);
            if (_archiveService.IsArchiveDirectory(archPath))
            {
                Archive archive = _archiveService.Read(archPath);
                Article[] articles = archive.Nodes.ToArray();
                Detail[] details = Array.ConvertAll<Article, Detail>(articles, a => a.Detail);
                for (int i = 0; i < details.Length; i++)
                {
                    Console.WriteLine(details[i][key]);
                }
            }
        }
        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 修改字段(string archPath, string key, string value)
        {
            archPath = GetTargetPath(archPath);
            if (_archiveService.IsArchiveDirectory(archPath))
            {
                Archive archive = _archiveService.Read(archPath);
                Article[] articles = archive.Nodes.ToArray();
                Detail[] details = Array.ConvertAll<Article, Detail>(articles, a => a.Detail);
                for (int i = 0; i < details.Length; i++)
                {
                    if(details[i][key] != null)
                    {
                        Console.WriteLine($"{details[i][key]} 替换 {value}");
                        _articleService.WriteTxtProperty(articles[i].TxtPath, key, value);
                    }
                }
            }
        }
        #endregion

        #region 条目操作


        [Status(StatuTypes.Article, CompleteTypes.Finish)]
        public void 创建条目(string archPath, string title)
        {
            archPath = GetTargetPath(archPath);
            if(_archiveService.IsArchiveDirectory(archPath))
            {
                string path = Path.Combine(archPath, title);
                if( !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        [Status(StatuTypes.Article, CompleteTypes.Finish)]
        public void 插入条目(string artiPath)
        {
            string archPath = Path.GetDirectoryName(artiPath);
            string name = Path.GetFileName(artiPath);
            if (Regex.IsMatch(name, @"^\d{2}\D?"))
            {
                string[] dirs = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly);
                string no = name.Substring(0, 2);
                for (int i = dirs.Length - 1; i >= 0; i--)
                {
                    string dirno = Path.GetFileName(dirs[i]).Substring(0, 2);
                    if (String.Compare(no, dirno) <= 0)
                    {
                        Directory.Move(dirs[i], Path.Combine(
                            Path.GetDirectoryName(dirs[i]),
                            (int.Parse(dirno) + 1).ToString("D2")
                            + Path.GetFileName(dirs[i]).Substring(2)));
                    }
                }

                Directory.CreateDirectory(Path.Combine(archPath, no));
            }
        }

        [Status(StatuTypes.Article, CompleteTypes.Finish)]
        public void 删除条目(string artiPath)
        {
            string artiName = Path.GetFileName(artiPath);
            string name = Path.GetFileName(artiPath);
            if (Regex.IsMatch(name, @"^\d{2}\D?"))    // 开头两个为数字
            {
                string archPath = Path.GetDirectoryName(artiPath);
                string[] dirs = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly);
                string no = artiName.Substring(0, 2);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dirno = Path.GetFileName(dirs[i]).Substring(0, 2);
                    if (String.Compare(no, dirno) == 0)
                    {
                        Directory.Delete(dirs[i], true);
                    }
                    else if (String.Compare(no, dirno) < 0)
                    {
                        Directory.Move(dirs[i], Path.Combine(
                            Path.GetDirectoryName(dirs[i]),
                            (int.Parse(dirno) - 1).ToString("D2")
                            + Path.GetFileName(dirs[i]).Substring(2)));
                    }
                }

            }
         }

        [Status(StatuTypes.Article, CompleteTypes.Debug)]
        public void 上移条目(string artiPath) { }
        public void 下移条目(string artiPath) { }
        #endregion

        #region 封面操作
        [Status(StatuTypes.Cover, CompleteTypes.Finish)]
        public void 创建封面(string archPath)
        {
            archPath = GetTargetPath(archPath);

            if (_archiveService.IsArchiveDirectory(archPath))
            {
                // 输出项目信息
                Console.WriteLine("项目列表：");
                for (int i = 0; i < _configurationService.ProjectSettings.Count; i++)
                {
                    Console.WriteLine("{0,-3:D} {1}", i + 1, _configurationService.ProjectSettings.ElementAt(i).Value.Describe);
                }

                Console.WriteLine("条例列表：");
                Archive archive = _archiveService.Read(archPath);
                Article[] articles = archive.Nodes?.ToArray(); ;
                for (int i = 0; i < articles.Length; i++)
                {
                    //if( articles[i].Detail.Title.Contains("协议") )
                    Console.WriteLine("{0,-3:D} {1:D4} {2} {3} {4} {5}",
                        i,
                        archive.Id,
                        articles[i].Detail.SubTitle,
                        articles[i].Detail.Title,
                        articles[i].Detail.Owner,
                        articles[i].Detail.Measure);
                }
                Console.Write("输入序号：");
                string str = Console.ReadLine();
                int nn = int.Parse(str.Split(' ')[0]);
                int nu = int.Parse(str.Split(' ')[1]);

                string projectName = _configurationService.ProjectSettings.ElementAt(nn - 1).Value.Describe;
                string path = Path.Combine(archPath, "征地档案封面.docx");
                string title = articles[nu].Detail.Title;
                string sub = articles[nu].Detail.SubTitle;
                string owner = articles[nu].Detail.Owner;
                string mesure = articles[nu].Detail.Measure;

                Console.WriteLine("生成Token：");
                CoverToken token = _wordService.BuildCoverToken(nn, archive.Id, projectName, title, sub, owner, mesure);
                Console.WriteLine(token.ToString());

                Console.Write("是否创建封面文档（Y/N）：");
                string key = Console.ReadLine();
                if (key.Equals("Y") || key.Equals("y"))
                {
                    _wordService.BuildCover(path, token);
                    Console.WriteLine("生成封面成功");
                }
            }


        }

        [Status(StatuTypes.Cover, CompleteTypes.Finish)]
        public void 拷贝封面(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archPaths = GetMaskArchivePaths(proPath, mask);
            string tmp_path = Path.Combine(proPath, "cover_tmp");
            if (!Directory.Exists(tmp_path))
            {
                Directory.CreateDirectory(tmp_path);
            }
            foreach (string arch in archPaths)
            {
                string cover = Directory.GetFiles(arch, "征地档案封面.doc*", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if( !string.IsNullOrEmpty(cover))
                {
                    Copy2Tmp(cover, tmp_path);
                }
            }
        }

        #endregion

        #region 目录操作
        [Status(StatuTypes.Catalog, CompleteTypes.Finish)]
        public void 拷贝目录(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archPaths = GetMaskArchivePaths(proPath, mask);
            string tmp_path = Path.Combine(proPath, "catalog_tmp");
            if (!Directory.Exists(tmp_path))
            {
                Directory.CreateDirectory(tmp_path);
            }
            foreach (string arch in archPaths)
            {
                string catalog = Directory.GetFiles(arch, "目录.doc*", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (!string.IsNullOrEmpty(catalog))
                {
                    Copy2Tmp(catalog, tmp_path);
                }
            }
        }

        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 读取目录首项(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archives = GetMaskArchivePaths(proPath, mask);
            foreach (string arch in archives)
            {
                Console.WriteLine("{0}", Path.GetFileName(arch).Substring(0, 4));

                Catalog[] catalogs = _wordService.ReadCatalog(Path.Combine(arch, "目录.doc"));
                for (int i = 0; i < catalogs.Length; i++)
                {
                    if ((bool)(catalogs[i]?.PageNumber?.StartsWith("1") ?? false))
                        Console.WriteLine("  {0}  {1}", catalogs[i].Title, catalogs[i].PageNumber);
                }
            }
        }

        [Status(StatuTypes.Catalog, CompleteTypes.Finish)]
        public void 创建目录(string archPath)
        {
            archPath = GetTargetPath(archPath);
            if (_archiveService.IsArchiveDirectory(archPath))
            {
                Archive archive = _archiveService.Read(archPath);
                Article[] articles = archive.Nodes.ToArray();
                Catalog[] catalogs = new Catalog[articles.Length];
                for (int i = 0; i < articles.Length; i++)
                {
                    Catalog catalog = new Catalog();
                    catalog.Title = articles[i].Detail.Title;
                    catalog.Number = articles[i].Detail.Dossier;
                    catalog.Date = articles[i].Detail.Year;
                    catalog.PageNumber = articles[i].Detail.PageNumber;
                    if (string.IsNullOrWhiteSpace(catalog.PageNumber)){
                        catalog.PageNumber = "0";
                    }
                    catalogs[i] = catalog;
                }

                catalogs = catalogs.OrderBy(c => int.Parse(c.PageNumber.Split('-')[0])).ToArray();


                Console.WriteLine("====================================================");
                for (int i = 0; i < catalogs.Length; i++)
                {
                    catalogs[i].Id = (i+1).ToString();
                    Console.WriteLine("{0,-2} - {1} - {2} - {3} - {4} - {5,-6} - {6}", 
                        catalogs[i].Id, 
                        catalogs[i].Title.PadLeftEx(50, ' '), 
                        catalogs[i].Author, 
                        catalogs[i].Date.PadLeftEx(10, ' '), 
                        catalogs[i].Number.PadLeftEx(40, ' '), 
                        catalogs[i].PageNumber, 
                        catalogs[i].Other);
                }
                Console.WriteLine("====================================================");
                // 档案号
                string archiveNumber = Path.GetFileName(archPath).Substring(0, 4);
                string proNumber = Directory.GetParent(archPath).Name;
                string num = string.Format("ZY•ZD•{0}•Y-{1:D3}", int.Parse(proNumber.Substring(2,3)), int.Parse(archiveNumber));
                Console.WriteLine($"档案号 {num}");
                Console.Write("是否创建目录文档（Y/N）：");
                string key = Console.ReadLine();
                if (key.Equals("Y") || key.Equals("y"))
                {
                    string path = Path.Combine(archPath, "目录.doc");
                    _wordService.BuildCatalog(path, num, catalogs, Console.WriteLine);
                }
            }

        }
        #endregion

        #region PDF
        [Status(StatuTypes.Pdf, CompleteTypes.Debug)]
        public void 提取PDF首页文字(string pdfPath)
        {
            if( File.Exists(pdfPath))
            {
                Console.WriteLine(_articleService.GetPdfTxtPage0(pdfPath));

            }
        }

        [Status(StatuTypes.Pdf, CompleteTypes.SubFunc)]
        public void 修改PDF名称按照TXT名称(string archPath)
        {
            int i = 0;
            archPath = GetTargetPath(archPath);
            string[] txts = Directory.GetFiles(archPath, "*.txt", SearchOption.AllDirectories).ToArray();
            foreach (string txt in txts)
            {
                string artiPath = Path.GetDirectoryName(txt);
                string txtName = Path.GetFileNameWithoutExtension(txt);
                string pdf = Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly).FirstOrDefault();
                string destPath = Path.Combine(artiPath, txtName + ".pdf");
                if ( !string.IsNullOrEmpty(pdf) && !File.Exists(destPath) )
                {
                    i++;
                    if( !File.Exists(destPath) )
                        File.Move(pdf, destPath);
                }
            }
            Console.WriteLine("修改 {0,2:D} 个PDF ...", i);
        }

        private string RemoveOther(string str)
        {
            str = str.Replace("（", "(").Replace("）", ")");
            int l = str.IndexOf('(');
            int r = str.LastIndexOf(')');
            string s = l>=0 ? str.Substring(0, l)  + str.Substring(r).Remove(0) : str;
            return s;
        }

        [Status(StatuTypes.Pdf, CompleteTypes.SubFunc)]
        public void 拆分PDF文件(string archPath)
        {
            string[] mutiplePdfDirs = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly)
                .Where(s => RemoveOther(Path.GetFileName(s)).Contains('-')).ToArray();

            foreach (string mutipleDir in mutiplePdfDirs)
            {
                string[] pdfs = Directory.GetFiles(mutipleDir, "*.pdf", SearchOption.TopDirectoryOnly).ToArray();
                if( pdfs.Length == 1)
                {
                    string pdf = pdfs[0];
                    string name = Path.GetFileName(pdf);
                    string dirName = Path.GetFileName(mutipleDir);
                    // 分割pdf
                    Console.Write("拆分 {0}../{1}...pdf ", dirName.Substring(0, 5), name.Substring(0, 5));
                    string[] splitPdfs = _articleService.SplitPdf(pdf);
                    Console.WriteLine("1-{0:D}", splitPdfs.Length);
                    // 分割pdf重命名
                    Console.Write("重命名被分割文件 ");
                    Catalog[] catalogs = _wordService.ReadCatalog(Path.Combine(archPath, "目录.doc"));
                    foreach (string p in splitPdfs)
                    {
                        string num = Path.GetFileNameWithoutExtension(p);
                        Console.Write("{0:D} ", num);
                        Catalog[] cs = catalogs.Where(c => c.PageNumber.Equals(num)).ToArray();
                        string title = cs.Length == 1 ? cs[0].Title : num;
                        string path = Path.Combine(mutipleDir, title + ".pdf");
                        if (!File.Exists(path))
                            File.Move(p, path);
                    }
                    Console.WriteLine("");
                    // 修改原始文件名字
                    string txtName = Directory.GetFiles(mutipleDir, "*.txt", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    txtName = Path.GetFileNameWithoutExtension(txtName);
                    Console.Write("修改原始文件 {0}...pdf to {1}...pdf",name.Substring(0, 5), txtName.Substring(0, 5));
                    string dst = Path.Combine(mutipleDir, txtName + ".pdf");
                    if (!File.Exists(dst))
                        File.Move(pdf, dst);
                    Console.WriteLine(".. OK");
                }
            }
        }

        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 修改拆分项目PDF文件(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archPaths = GetMaskArchivePaths(proPath, mask);
            foreach (string arch in archPaths)
            {
                Console.WriteLine("{0,4} ", Path.GetFileName(arch).Substring(0, 4));
                修改PDF名称按照TXT名称(arch);
                拆分PDF文件(arch);
            }
        }

        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 按照文件夹名称拷贝PDF(string archPath)
        {
            archPath = GetTargetPath(archPath);
            if( _archiveService.IsArchiveDirectory(archPath))
            {
                string[] artiPaths = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly).ToArray();
                artiPaths = artiPaths.Where(a => Regex.IsMatch(Path.GetFileName(a), @"^\d{2}?")).ToArray();
                string[] pdfs = Directory.GetFiles(archPath, "*.pdf", SearchOption.TopDirectoryOnly).ToArray();
                foreach (string arti in artiPaths)
                {
                    string num = Path.GetFileName(arti).Substring(0, 2);
                    Console.Write("[");
                    ConsoleColor currentForeColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{num}");
                    Console.ForegroundColor = currentForeColor;
                    Console.Write("] ");
                }
                Console.WriteLine();
                foreach (string pdf in pdfs)
                {
                    string num = Path.GetFileName(pdf).Substring(0, 2);
                    Console.Write("[");
                    ConsoleColor currentForeColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{num}");
                    Console.ForegroundColor = currentForeColor;
                    Console.Write("] ");
                }
                Console.WriteLine();
                Console.Write("是否拷贝pdf（Y/N）：");
                string key = Console.ReadLine();
                if (key.Equals("Y") || key.Equals("y"))
                {
                    foreach (string arti in artiPaths)
                    {
                        ArticleToken articleToken = _articleService.GetArticleToken(arti);
                        string name = articleToken.Name; 
                        string num = Path.GetFileName(arti).Substring(0,2);
                        File.Move(Path.Combine(archPath, $"{num}.pdf"), Path.Combine(arti, $"{name}.pdf"));
                        Console.Write("[");
                        ConsoleColor currentForeColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{num}");
                        Console.ForegroundColor = currentForeColor;
                        Console.Write("] ");
                    }
                    Console.WriteLine();
                 }
            }
        }

       private void mergePDFs(params string[] pdfs)
        {
            _articleService.MergePdf(pdfs.Skip(0).Take(pdfs.Length - 1).ToArray(), pdfs.Last());
        }

        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 合并PDF2(string pdf0, string pdf1, string pdf2)
            => mergePDFs(pdf0, pdf1, pdf2);
        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 合并PDF3(string pdf0, string pdf1, string pdf2, string pdf3) 
            => mergePDFs(pdf0, pdf1, pdf2, pdf3);
        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 合并PDF4(string pdf0, string pdf1, string pdf2, string pdf3, string pdf4) 
            => mergePDFs(pdf0, pdf1, pdf2, pdf3, pdf4);
        [Status(StatuTypes.Pdf, CompleteTypes.Finish)]
        public void 合并PDF5(string pdf0, string pdf1, string pdf2, string pdf3, string pdf4, string pdf5) 
            => mergePDFs(pdf0, pdf1, pdf2, pdf3, pdf4, pdf5);
        #endregion

        #region 档案操作
        [Status(StatuTypes.Archive, CompleteTypes.Finish, "修改档案编号 ./0024-... '0025'")]
        public void 修改档案编号(string srcArchPath, string num)
        {
            if (_archiveService.IsArchiveDirectory(srcArchPath))
            {
                string src = srcArchPath;
                string dst = num;

                string[] txts = Directory.GetFiles(src, "*.txt", SearchOption.AllDirectories);
                foreach (string txt in txts)
                {
                    string v = _articleService.ReadTxtProperty(txt, "案卷号");
                    string d = v.Substring(0, 5) + dst;
                    _articleService.WriteTxtProperty(txt, "案卷号", d);
                }

                string dst_dir = Path.Combine(Path.GetDirectoryName(src), dst + Path.GetFileName(src).Substring(4));
                Console.WriteLine(dst_dir);
                Directory.Move(src, dst_dir);
            }
        }


        [Status(StatuTypes.Archive, CompleteTypes.Finish)]
        public void 拷贝协议书(string proPath, string mask)
        {
            string[] archPaths = GetMaskArchivePaths(proPath, mask);
            string tmp_path = Path.Combine(proPath, "dossier_tmp");
            if (!Directory.Exists(tmp_path))
            {
                Directory.CreateDirectory(tmp_path);
            }
            foreach (string arch in archPaths)
            {
                string[] txts = Directory.GetFiles(arch, "*.txt", SearchOption.AllDirectories);
                foreach (string txt in txts)
                {
                    Detail detail = _articleService.ReadTxtProperties(txt);
                    if (!string.IsNullOrWhiteSpace(detail.Dossier) && detail.Title.Contains("协议书"))
                    {
                        string artiPath = Path.GetDirectoryName(txt);
                        string pdf = Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly).FirstOrDefault();
                        Console.WriteLine($"拷贝 {Path.GetFileName(Path.GetDirectoryName(artiPath)).Substring(0, 4)} 协议书 {detail.Dossier.Trim()}.pdf ...");
                        File.Copy(pdf, Path.Combine(tmp_path, detail.Dossier.Trim()) + ".pdf", true);
                    }
                }
            }
        }

        [Status(StatuTypes.Archive, CompleteTypes.Finish)]
        public void 档案目录添加权属人(string archPath)
        {
            string txt = Directory.GetFiles(archPath, "*.txt", SearchOption.AllDirectories).First();
            string name = _articleService.ReadTxtProperty(txt, "权属人");
            string archName = RemoveOther(Path.GetFileName(archPath));
            string path = Path.Combine(Path.GetDirectoryName(archPath),
                archName + $"（{name}）");

            Console.WriteLine("修改档案文件夹名...");
            if (!Directory.Exists(path))
                Directory.Move(archPath, path);
        }
        #endregion

        #region 其他


        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 打印协议书信息(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] dirs = GetMaskArchivePaths(proPath, mask);
            foreach (string dir in dirs)
            {
                ArchvieTitleContainsAgreement(dir, d =>
                {
                    Console.WriteLine(
                        $" {d.Measure.PadRightEx(30, ' ')}" +
                        $"{d.Owner.PadRightEx(30, ' ')}" +
                        $"{d.Dossier.PadRightEx(30, ' ')}" +
                        $"{d.SubTitle.PadRightEx(50, ' ')}{d.Title}");
                });
            }
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 打印协议书权属人测量编号(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] dirs = GetMaskArchivePaths(proPath, mask);

            foreach (string dir in dirs)
            {
                ArchvieTitleContainsAgreement(dir, d =>
                {
                    Console.WriteLine($" {d.Owner}，{d.Measure}".PadRightEx(50, ' ') + $"{d.SubTitle}{d.Title}");
                });
            }
        }
        
        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 打印协议书txt文件路径(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archives = GetMaskArchivePaths(proPath, mask);
            foreach (string arch in archives)
            {
                //Console.Write(Path.GetFileName(arch).Substring(0, 4));
                Archive archive = _archiveService.Read(arch);
                Article[] articles = archive.Nodes.Where(a => a.Detail.Title.Contains("协议书") || a.Detail.Title.Contains("合同")).ToArray();
                var groups = articles.GroupBy(a => a.TxtPath);
                foreach (var g in groups)
                {
                    Console.WriteLine(g.Key);
                }
            }
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 统计(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archives = GetMaskArchivePaths(proPath, mask);
            string proName = Path.GetFileName(proPath).Substring(0, 5);
            int scanAll = 0;
            int sumAll = 0;
            List<ReportMsg> msgs = new List<ReportMsg>();

            // 读取数据放在 msgs 中.
            Console.Write("读取页数 ...");
            foreach (string arch in archives)
            {
                int scan = 0;
                int sum = 0;
                Archive archive = _archiveService.Read(arch);
                Console.Write("{0:D4}/{1:D4}", archive.Id, archives.Length);
                foreach ( Article article in archive.Nodes)
                {
                    Detail detail = article.Detail;
                    if( detail != null)
                    {
                        int page, copies;
                        if( int.TryParse(detail.Pages, out page))
                        {
                            scan += page;
                            if( int.TryParse(detail.Copies, out copies))
                            {
                                sum += page * copies;
                            }
                        }
                    }
                }
                scanAll += scan;
                sumAll += sum;
                msgs.Add(new ReportMsg() { Id = archive.Id, Scan = scan, Sum = sum });
                Console.Write("\b\b\b\b\b\b\b\b\b");
            }
            Console.WriteLine();

            ConsoleKey key = ConsoleKey.T;
            int start = 0;
            do
            {
                Console.Clear();
                switch (key)
                {
                    case ConsoleKey.LeftArrow: 
                        {
                            if (start >= 30) start -= 30;
                        }  
                        break;
                    case ConsoleKey.RightArrow: 
                        {
                            if (start <= (msgs.Count - 30)) start += 30;
                        }
                        break;
                    case ConsoleKey.T:    // 表格显示
                        {

                        }
                        break;
                    case ConsoleKey.L:    // 列表显示
                        {
                            foreach (ReportMsg msg in msgs)
                            {
                                Console.WriteLine("{0:D4} {1,5:D} {2,5:D}", msg.Id, msg.Scan, msg.Sum);
                            }
                        }
                        break;
                    case ConsoleKey.P:    // 导出 xlsx
                        {
                            Console.WriteLine("当前目录：{0}", proPath);
                            string name;
                            if(Yes("是否按照项目路径生成xlsx"))
                            {
                                name = _configurationService.ProjectSettings.ContainsKey(proName) ?
                                    _configurationService.ProjectSettings[proName].Describe : null;
                            }
                            else
                            {
                                Console.Write("输入标题：");
                                name = Console.ReadLine();
                            }
                            Console.WriteLine("标题：{0}，档案份数：{1}，扫描页数{2}，总页数：{3}", name,  msgs.Count,scanAll, sumAll);
                            string path = Path.Combine(proPath, "统计表" + DateTime.Now.ToString("yyyy-MM-dd[HH-mm-ss]") + ".xlsx");
                            Console.WriteLine("文件路径：{0}", path);
                            if (Yes("是否生成"))
                            {
                                msgs.Add(new ReportMsg() { Id = 9999, Scan = scanAll, Sum = sumAll });
                                _reportService.SaveToXLSX(path, msgs.ToArray(), name);
                                msgs.RemoveAt(msgs.Count - 1);
                            }                     
                        }
                        break;
                    default: break;
                }

                ReportMsg[] ms1 = new ReportMsg[10];
                ReportMsg[] ms2 = new ReportMsg[10];
                ReportMsg[] ms3 = new ReportMsg[10];
                msgs.Skip(start).Take(10).ToArray().CopyTo(ms1, 0);
                msgs.Skip(start + 10).Take(10).ToArray().CopyTo(ms2, 0);
                msgs.Skip(start + 20).Take(10).ToArray().CopyTo(ms3, 0);
                Console.WriteLine("+-----------+-----+-----+-----------+-----+-----+-----------+-----+-----+");
                for (int i = 0; i < 10; i++)
                {

                    Console.WriteLine("| " + proName + "{0,4:D4} |{1,5:D}|{2,5:D}| "
                        + proName + "{3,4:D4} |{4,5:D}|{5,5:D}| "
                        + proName + "{6,4:D4} |{7,5:D}|{8,5:D}|",
                        ms1[i]?.Id, ms1[i]?.Scan, ms1[i]?.Sum,
                        ms2[i]?.Id, ms2[i]?.Scan, ms2[i]?.Sum,
                        ms3[i]?.Id, ms3[i]?.Scan, ms3[i]?.Sum);
                    Console.WriteLine("+-----------+-----+-----+-----------+-----+-----+-----------+-----+-----+");
                }
                Console.WriteLine("份数：{0} 扫描：{1} 总数：{2}", msgs.Count, scanAll, sumAll);

                Console.Write("输入[←，→，ESC，T(表格)，L(列表)，P(导出xlsx)]:");
                
            } while ((key = Console.ReadKey().Key) != ConsoleKey.Escape);
        
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 逆向(string archPath)
        {
            archPath = GetTargetPath(archPath);

            if (_archiveService.IsArchiveDirectory(archPath))
            {
                string[] pdfs = Directory.GetFiles(archPath, "*.pdf", SearchOption.AllDirectories);
                foreach (string pdf in pdfs)
                {
                    string parent = Path.GetFileName(Path.GetDirectoryName(pdf));
                    string name = Path.GetFileName(pdf);
                    if( Regex.IsMatch(parent, @"\d{2}\D*"))
                    {
                        File.Move(pdf, Path.Combine(archPath, parent.Substring(0, 2)) + ".pdf");
                        Console.WriteLine($"move {name} ../{parent.Substring(0, 2)}");
                    }
                }
                删除TXT文件(archPath);
            }
        }

        private void ColorMsgLine(string str, string mask, ConsoleColor color)
        {
            ConsoleColor curForeColor = Console.ForegroundColor;
            Console.Write(str);
            Console.ForegroundColor = color;
            Console.WriteLine(mask);
            Console.ForegroundColor = curForeColor;
        }
        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 检查完成情况(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archs = GetMaskArchivePaths(proPath, mask);

            foreach (string arch in archs)
            {   
                string name = Path.GetFileName(arch);
                ConsoleColor curForeColor = Console.ForegroundColor;
                bool flag = false;
                bool hasCover = File.Exists(Path.Combine(arch, "征地档案封面.docx"));
                bool hasCatalog = File.Exists(Path.Combine(arch, "征地档案封面.docx"));


                Console.Write(name.Substring(0,4));

                string[] dirs = Directory.GetDirectories(name, "*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    int txtLength = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly).Length;
                    int pdfLength = Directory.GetFiles(dir, "*.pdf", SearchOption.TopDirectoryOnly).Length;
                   
                    if( ( txtLength != 1 || pdfLength != 1) && !flag)
                    {
                            Console.Write("\b\b\b\b");
                            Console.WriteLine(name);
                            flag = true;
                    }
                        
                    if (txtLength != 1)
                    {
                       
                        Console.WriteLine($"    - {Path.GetFileName(dir)}");
                        ColorMsgLine($"        - TXT", "X", ConsoleColor.Red);
                    }
                    if (pdfLength != 1)
                    {

                        Console.WriteLine($"    - {Path.GetFileName(dir)}");
                        ColorMsgLine($"        - PDF", "X", ConsoleColor.Red);
                    }
                }

                if ((!hasCatalog || !hasCover) && !flag)
                {
                    Console.Write("\b\b\b\b");
                    Console.WriteLine(name);
                    flag = true;
                }
                if (!hasCatalog)
                {
                    ColorMsgLine($"    - 目录", "X", ConsoleColor.Red);
                }
                if (!hasCover)
                {
                    ColorMsgLine($"    - 封面", "X", ConsoleColor.Red);
                }
                if ( !flag )
                {
                    Console.Write("\b\b\b\b");
                }
                
            }
        }

        [Status(StatuTypes.Other, CompleteTypes.SubFunc)]
        public void 命令帮助(MethodInfo methodInfo)
        {
            StatusAttribute attribute = methodInfo.GetCustomAttribute(typeof(StatusAttribute)) as StatusAttribute;
            Console.Write("[");
            ConsoleColor currentForeColor = Console.ForegroundColor;
            switch (attribute.Complete)
            {
                case CompleteTypes.Finish:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("{0}", "F");
                    break;
                case CompleteTypes.Debug:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("{0}", "D");
                    break;
                case CompleteTypes.SubFunc:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("{0}", "S");
                    break;
                default: break;
            }
            Console.ForegroundColor = currentForeColor;
            Console.Write("] ");


            Console.Write("{0,-15}", methodInfo.Name.PadRightEx(30, ' '));
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach (ParameterInfo info in parameterInfos)
            {
                Console.Write(", {0} {1}", info.ParameterType.Name, info.Name);
            }
            if( !string.IsNullOrWhiteSpace(attribute.Example))
            {
                Console.Write($", {attribute.Example}");
            }
            Console.WriteLine("");
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 帮助()
        {
            MethodInfo[] methodInfos = typeof(Util).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            methodInfos = methodInfos.Where(m => m.GetCustomAttribute<StatusAttribute>() != null).ToArray();
            var enumerables = methodInfos.GroupBy(m => m.GetCustomAttribute<StatusAttribute>().Type).ToArray();

            #region 标题
            ConsoleColor curForeColor = Console.ForegroundColor;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("F");
            Console.ForegroundColor = curForeColor;
            Console.Write("]已完成可直接使用; ");
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("S");
            Console.ForegroundColor = curForeColor;
            Console.Write("]已完成子功能; ");
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("D");
            Console.ForegroundColor = curForeColor;
            Console.WriteLine("]未完成，开发中; ");
            #endregion

            foreach (var item in enumerables)
            {
                Console.WriteLine($"------------------");
                foreach (var methodInfo in item)
                {
                    命令帮助(methodInfo);
                }
            }

        }

        #endregion




        [Status(StatuTypes.Other, CompleteTypes.Debug)]
        public void 测试()
        {
            
        }
    }
}
