using BlankApp.Service;
using BlankApp.Service.Extensions;
using BlankApp.Service.Impl;
using BlankApp.Service.Model;
using BlankApp.Service.Model.Object;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlankApp.Cli
{
    public class Util
    {
        private readonly IArticleService _articleService;
        private readonly IConfigurationService _configurationService;
        private readonly IMaskService _maskService;
        private readonly IWordService _wordService;
        private readonly IArchiveService _archiveService;

        public Util(IArticleService articleService, IConfigurationService configurationService, IMaskService maskService, IWordService wordService, IArchiveService archiveService)
        {
            this._articleService = articleService;
            this._configurationService = configurationService;
            this._maskService = maskService;
            this._wordService = wordService;
            this._archiveService = archiveService;
        }
        #region private
        private string[] GetMaskArchivePaths(string proPath, string mask)
        {
            Console.Write("获取目录...");
            string[] archPaths = Directory.GetDirectories(proPath, "*", SearchOption.TopDirectoryOnly).ToArray();
            Console.Write("\b\b\b\b\b\b\b\b\b\b\b过滤目录...");
            archPaths = archPaths.Where(_archiveService.IsArchiveDirectory).ToArray();
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
            // 开头为 '.' 替换为当前路径
            if (simplePath.StartsWith(".") && !simplePath.StartsWith(".."))
                simplePath = simplePath.Remove(0).Insert(0, Directory.GetCurrentDirectory());

            // 开头为 '..' 替换为上一层目录
            if (simplePath.StartsWith("..") && !simplePath.StartsWith("..."))
                simplePath = simplePath.Remove(0, 2).Insert(0, Path.GetDirectoryName(Directory.GetCurrentDirectory()));

            // 结尾为 '/' 删除
            if (simplePath.EndsWith($"{Path.DirectorySeparatorChar}"))
                simplePath = simplePath.Remove(simplePath.Length - 1);

            return simplePath;
        }

        private void Copy2Tmp(string src, string tmpPath)
        {
            string archPath = Path.GetDirectoryName(src);
            string num = Path.GetFileName(archPath).Substring(0, 4);
            Console.WriteLine($"拷贝 {num} {Path.GetFileName(src)} ...");
            File.Copy(src, Path.Combine(tmpPath, $"{num}-{Path.GetFileName(src)}"), true);
        }
        #endregion

        #region txt文件操作

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 创建独立txt文件(string artiPath, string 测量编号, string 权属人, string 案卷号)
        {
            artiPath = GetTargetPath(artiPath);

            string[] pdfs = Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly);
            string pdf = pdfs.Length > 0 ? pdfs[0] : null;
            if( !string.IsNullOrEmpty(pdf) )
            {
                //string artiPath = Path.GetDirectoryName(pdf);

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
                //读pdf页数
                int page = 0;
                using (PdfReader pdfReader = new PdfReader(pdf))
                {
                    page = pdfReader.NumberOfPages;
                }


                Console.WriteLine($"路径:{txtPath}");
                Console.WriteLine($"名称:{Path.GetFileNameWithoutExtension(txtPath)}");
                Console.WriteLine($"权属人:{权属人}");
                Console.WriteLine($"测量:{测量编号}");
                Console.WriteLine($"页数:{page}");
                Console.WriteLine($"份数:{copies}");
                Console.WriteLine($"案卷号:{案卷号}");
                Console.WriteLine();


                Detail detail = new Detail();
                detail.Title = Path.GetFileNameWithoutExtension(txtPath);
                detail.Owner = 权属人;
                detail.Number = 案卷号;
                detail.Copies = copies;
                detail.Measure = 测量编号;
                detail.Pages = page > 0 ? page.ToString() : "";
                _articleService.WriteTxtProperties(txtPath, detail);
            }
        }

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 创建txt文件(string archPath, string 测量编号, string 权属人, string 案卷号)
        {
            archPath = GetTargetPath(archPath);

            if( _archiveService.IsArchiveDirectory(archPath))
            {
                string[] artiPaths = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly);
                foreach (string arti in artiPaths)
                {
                    创建独立txt文件(arti, 测量编号, 权属人, 案卷号);
                }
            }
            
        }
       

        [Status(StatuTypes.Txt, CompleteTypes.Finish)]
        public void 删除txt文件(string archPath)
        {
            archPath = GetTargetPath(archPath);
            if( _archiveService.IsArchiveDirectory(archPath))
            {
                string[] files = Directory.GetFiles(archPath, "*.txt", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Console.WriteLine("删除成功");
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

        #region 封面操作
        [Status(StatuTypes.Cover, CompleteTypes.Finish)]
        public void 创建封面(string archPath)
        {
            archPath = GetTargetPath(archPath);

            if (_archiveService.IsArchiveDirectory(archPath))
            {
                // 输出项目信息
                Console.WriteLine("项目列表：");
                ProjectObject[] projectObjects = _configurationService["Projects"] as ProjectObject[];
                for (int i = 0; i < projectObjects.Length; i++)
                {
                    Console.WriteLine("{0,-3:D} {1}", i + 1, projectObjects[i].Describe);
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

                string projectName = projectObjects[nn - 1].Describe;
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

        [Status(StatuTypes.Cover, CompleteTypes.Debug)]
        public void 打印封面(string archPath)
        {
            if (archPath.Equals("."))
                archPath = Directory.GetCurrentDirectory();

            if (_archiveService.IsArchiveDirectory(archPath))
            {
                string coverPath = Path.Combine(archPath, "征地档案封面.doc");
                if (File.Exists(coverPath))
                {
                    _wordService.PrintCover(coverPath);
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
        #endregion

        #region PDF
        [Status(StatuTypes.Pdf, CompleteTypes.Debug)]
        public void 提取PDF首页文字(string artiPath)
        {
            if (artiPath.Equals("."))
                artiPath = Directory.GetCurrentDirectory();
            if( _articleService.IsArticleDirectory(artiPath))
            {
                Console.WriteLine(_articleService.GetPdfTxtPage0(artiPath));
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
            // string destName = "单张合并电子版.pdf";
            
            // string[] pdfs = Directory.GetFiles(archPath, "*.pdf", SearchOption.AllDirectories).ToArray();
            string[] mutiplePdfDirs = Directory.GetDirectories(archPath, "*", SearchOption.TopDirectoryOnly)
                .Where(s => RemoveOther(Path.GetFileName(s)).Contains('-')).ToArray();
            // string[] finishPdf = pdfs.Where(s =>  s.EndsWith(destName) ).ToArray();
            // string[] finishPdfDirs = Array.ConvertAll<string, string>(finishPdf, Path.GetDirectoryName).Distinct().ToArray();
            // string[] mutiplePdfs = pdfs.Where(s => Path.GetFileName(s).Contains('-') && !finishPdfDirs.Contains(Path.GetDirectoryName(s))).ToArray();
            // string[] mutiplePdfs = pdfs.Where(s => Path.GetFileName(s).Contains('-') ).ToArray();
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
        #endregion

        #region 档案操作
        [Status(StatuTypes.Archive, CompleteTypes.Finish)]
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
                    //Console.WriteLine(src, ReadTxtProperty(txt, "案卷号"));
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

        #region 检查错误        
        private string CheckArchiveDirectoryName(string archPath)
        {
            string name = Path.GetFileName(archPath);
            
            if (name.Contains("-") && name.Split('-').Length > 2) return name;
            if (name.Contains(" ")) return name;
            if (name.Contains("（") || name.Contains("）")) return name;

            return null;
        }
        private string CheckArticleDirectoryName(string artiPath)
        {
            string name = Path.GetFileName(artiPath);

            if (name.Contains(" ")) return name;
            if (name.Contains("（") || name.Contains("）")) return name;

            return null;
        }

        private string[] CheckMultiplePdf(string artiPath)
        {
            string[] pdfs = Directory.GetFiles(artiPath, "*.pdf", SearchOption.TopDirectoryOnly).ToArray();
            if( pdfs.Length > 0)
            {
                pdfs = Array.ConvertAll<string, string>(pdfs, Path.GetFileName);
            }

            return pdfs;
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 检查错误(string proPath, string mask)
        {
            string[] archives = GetMaskArchivePaths(GetTargetPath(proPath), mask);  
            Console.WriteLine("检查档案目录: ----");
            foreach (string arch in archives)
            {
                string m = CheckArchiveDirectoryName(arch);
                if( m != null)
                {
                    Console.WriteLine(m);
                }
            }

            Console.WriteLine("检查文章目录: ---");
            foreach (string arch in archives)
            {
                string msg = "";
                Console.Write(Path.GetFileName(arch).Substring(0, 4));
                foreach (string arti in Directory.GetDirectories(arch, "*", SearchOption.AllDirectories))
                {
                    string m =  CheckArticleDirectoryName(arti);
                    if( m != null)
                    {
                        msg += "  " + m + "\r\n";
                    }
                }
                Console.Write("\b\b\b\b");
                if( !string.IsNullOrWhiteSpace(msg))
                {
                    Console.WriteLine(Path.GetFileName(arch) + "\r\n" + msg);
                }
            }

            Console.WriteLine("检查PDF: ---");
            foreach (string arch in archives)
            {
                string msg = "";
                Console.Write(Path.GetFileName(arch).Substring(0, 4));
                foreach (string arti in Directory.GetDirectories(arch, "*", SearchOption.AllDirectories).ToArray())
                {
                    string[] pdfs = CheckMultiplePdf(arti);
                    if(pdfs.Length != 1)
                    {
                        msg += "  " + Path.GetFileName(arti) + ": " + pdfs.Length;
                    }
                }
                Console.Write("\b\b\b\b");
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Console.WriteLine(Path.GetFileName(arch) + "\r\n" + msg);
                }
            }

        }
        #endregion




        [Status(StatuTypes.Other, CompleteTypes.Debug)]
        public void 补全(string str)
        {
            string name = this._maskService.Complex(str);
            Console.WriteLine(name);
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 打印协议书信息(string proPath)
        {
            string[] dirs = Directory.GetDirectories(proPath, "*协议书*", SearchOption.AllDirectories);
            foreach (string dir in dirs)
            {
                string txt = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if( !string.IsNullOrEmpty(txt))
                {
                    Detail detail = _articleService.ReadTxtProperties(txt);
                    string number = detail["案卷号"];
                    string mesure = detail["测量编号"];
                    string owner = detail["权属人"];
                    string title = detail["提名"];
                    string sub_title = detail["副题名"];
                    Console.WriteLine("{0,10} {1} {2,-30} {3,15}", number, (mesure+"，"+owner).PadRightEx(40, ' '), title, sub_title);
                }
            }
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 打印协议号(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archives = GetMaskArchivePaths(proPath, mask);

      
            foreach (string arch in archives)
            {
                
                Console.Write(Path.GetFileName(arch).Substring(0, 4));
                Archive archive = _archiveService.Read(arch);
                Article[] articles = archive.Nodes.Where(a => a.Detail.Title.Contains("协议书") || a.Detail.Title.Contains("合同")).ToArray();
                //string[] txts = Directory.GetFiles(arch, "*.txt", SearchOption.AllDirectories)
                //    .Where(t => Path.GetFileName(t).Contains("协议书") || Path.GetFileName(t).Contains("合同")).ToArray();
                Console.Write("\b\b\b\b");
                /*if ( txts.Length > 0)
                {
                    foreach (string txt in txts)
                    {
                        Detail detail = _articleService.ReadTxtProperties(txt);
                        Console.WriteLine($"{detail.Number.Substring(5, 4)} {detail.Dossier} {detail.Title}");

                    }
                }*/
                bool flag = false;
                foreach (var a in articles.Where( a=> !string.IsNullOrWhiteSpace(a.Detail.Dossier)))
                {
                    if (!flag) 
                    {
                        Console.Write($"{a.Detail.Number.Substring(5, 4)}");
                        flag = true;
                    }
                    else
                    {
                        Console.Write("    ");
                    }
                    Console.WriteLine($" {a.Detail.Dossier.PadRightEx(30, ' ')} {a.Detail.Measure.PadRightEx(25, ' ')} {a.Detail.SubTitle}{a.Detail.Title}");
                }

                
            }
        }


        [Status(StatuTypes.Other, CompleteTypes.Debug)]
        public void 打印标题(string proPath, string mask, string configPath)
        {
            proPath = GetTargetPath(proPath);
            configPath = GetTargetPath(configPath);
            string[] archPaths = GetMaskArchivePaths(proPath, mask);

            // 获取配置
            WidthPair[] widthPairs = _configurationService.ReadWidthPairs("");
            var enumerable = widthPairs.GroupBy(w => w.Width).ToArray();
            var width3 = enumerable.Where(e => e.Key == 3).First().ToArray();
            var width4 = enumerable.Where(e => e.Key == 4).First().ToArray();
            var width6 = enumerable.Where(e => e.Key == 6).First().ToArray();
            var width8 = enumerable.Where(e => e.Key == 8).First().ToArray();

            foreach (var item in enumerable)
            {
                string demoName;
                switch (item.Key)
                {
                    case 3: demoName = "案卷提名3x11x14.docx"; break;
                    case 4: demoName = "案卷提名4x11x10.docx"; break;
                    case 6: demoName = "案卷提名6x11x6.docx"; break;
                    case 8: demoName = "案卷提名8x11x4.docx"; break;
                    default: break;
                }
                foreach (var widthPair in item)
                {

                }
               
            }


/*            for (int i = 0; i < keys.Length; i++)
            {
                
            }



            if (_archiveService.IsArchiveDirectory(archPath))
            {
                string coverPath = Path.Combine(archPath, "提名.doc");
                
                    //_wordService.BuildTitle(coverPath);
                
            }*/
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 统计(string proPath, string mask)
        {
            proPath = GetTargetPath(proPath);
            string[] archives = GetMaskArchivePaths(proPath, mask);
            Console.WriteLine("编号 扫描页 总页数");
            foreach (string arch in archives)
            {
                int scan = 0;
                int sum = 0;
                Archive archive = _archiveService.Read(arch);
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
                Console.WriteLine("{0:D4} {1,6:D} {2,6:D}", archive.Id, scan, sum);
            }
        }

        [Status(StatuTypes.Other, CompleteTypes.Finish)]
        public void 帮助()
        {
            MethodInfo[] methodInfos = typeof(Util).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var enumerables = methodInfos.GroupBy(m => (m.GetCustomAttribute(typeof(StatusAttribute)) as StatusAttribute).Type).ToArray();

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
                    Console.WriteLine("");
                }
            }

        }


    }
}
