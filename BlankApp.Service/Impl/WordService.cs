using Aspose.Words;
using BlankApp.Service.Extensions;
using BlankApp.Service.Model;
using NPOI.XWPF.UserModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

namespace BlankApp.Service.Impl
{
    public class WordService : IWordService
    {
        private IArchiveService _as;
        private IConfigurationService _cs;

        public WordService(IArchiveService archiveService, IConfigurationService configurationService)
        {
            this._as = archiveService;
            this._cs = configurationService;
        }

        public void CreateNominate()
        {
/*            Application oWord = new Application();
            Document oWordDoc = new Document();
            object path = "F:\\档案\\test.doc";

            Object Nothing = Missing.Value;
            oWord.Visible = true;
            oWordDoc = oWord.Documents.Add();

            Shape s = oWordDoc.Shapes.AddShape((int)Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 0, 100, 100);
            s.Fill.OneColorGradient(Microsoft.Office.Core.MsoGradientStyle.msoGradientFromCenter, 4, 1);
            s.TextFrame.ContainingRange.Text = "Blah";
            s.TextFrame.ContainingRange.Font.Size = 18.0f;
            s.TextFrame.ContainingRange.Orientation = WdTextOrientation.wdTextOrientationDownward;
            object format = WdSaveFormat.wdFormatDocument;
            oWordDoc.SaveAs(ref path, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
            oWordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
            oWord.Quit(ref Nothing, ref Nothing, ref Nothing);*/


        }

        public CoverToken BuildCoverToken(int projectNumber, int number, string projectName, string title, string subTitle, string owner, string measure)
        {

            string other = "";
            string project = projectName.Substring(0, projectName.IndexOf("项目"));
            if (project.Length <= 18)
            {
                project = (project + "项目").PadRightEx(40, ' ');
            }
            else
            {
                project = project.Substring(0, 19);
                other = project.Substring(19) + "项目";
                project = project.PadRightEx(40, ' ');
            }

            string[] titles = new string[3] { "", "", "" };
            string name = (other + subTitle + title).Replace('(', '（').Replace(')', '）');
            string no = ($"({ measure }" + (string.IsNullOrWhiteSpace(measure) ? "" : "，") + $"{owner})").Replace(" ", "");
            for (int i = 0; i < name.Length; i++)
            {
                if (i < 23)
                {
                    titles[0] += name[i];
                }
                else if (i < 46)
                {
                    titles[1] += name[i];
                }
            }
            titles[2] = no;

            CoverToken token = new CoverToken();
            token.ProjectNumber = projectNumber.ToString();
            token.Number = string.Format("{0:D3}", number);
            token.Titles = new string[4];
            token.Titles[0] = project;
            token.Titles[1] = titles[0].PadRightEx(48, ' ');
            token.Titles[2] = titles[1].PadRightEx(48, ' ');
            token.Titles[3] = titles[2].PadRightEx(48, ' ');
            return token;
        }




        public void BuildCover(string coverPath, CoverToken token)
        {

            // 加载模板
            string demoPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            demoPath = Path.Combine(demoPath, "Template", "封面.docx");
            using (FileStream fs = new FileStream(demoPath, FileMode.Open, FileAccess.Read))
            {
                XWPFDocument doc = new XWPFDocument(fs);
                IList<XWPFParagraph> paragraphs = doc.Paragraphs;
                paragraphs.Where(p => p.Text.Contains("$ProjectNumber$")).First().ReplaceText("$ProjectNumber$", token.ProjectNumber);
                paragraphs.Where(p => p.Text.Contains("$Number$")).First().ReplaceText("$Number$", token.Number);
                paragraphs.Where(p => p.Text.Contains("$Title$")).First().ReplaceText("$Title$", token.Titles[0]);
                XWPFParagraph[] ps = paragraphs.Where(p => p.Text.Contains("$TitleLine$")).ToArray();
                ps[0].ReplaceText("$TitleLine$", token.Titles[1]);
                ps[1].ReplaceText("$TitleLine$", token.Titles[2]);
                ps[2].ReplaceText("$TitleLine$", token.Titles[3]);
                FileStream dst = new FileStream(coverPath, FileMode.CreateNew, FileAccess.ReadWrite);
                doc.Write(dst);
                dst.Close();
                doc.Close();
            }

            //Spire.Doc.Document doc = new Spire.Doc.Document();
            //doc.LoadFromFile(demoPath, Spire.Doc.FileFormat.Doc);

            // 修改档案号
            //Spire.Doc.Documents.TextSelection[] textProjectNumbers = doc.FindAllString("$ProjectNumber$", true, true);
            //textProjectNumbers[0].GetAsOneRange().Text = token.ProjectNumber;
            //Spire.Doc.Documents.TextSelection[] textArchivesNumbers = doc.FindAllString("$Number$", true, true);
            //textArchivesNumbers[0].GetAsOneRange().Text = token.Number;



            // 项目名
            // doc.FindAllString("$Title$", true, true)[0].GetAsOneRange().Text = token.Titles[0];
            // 标题
            // Spire.Doc.Documents.TextSelection[] textTitleLines = doc.FindAllString("$TitleLine$", true, true);
            // for (int i = 0; i < textTitleLines.Length; i++)
            // {
            //     textTitleLines[i].GetAsOneRange().Text = token.Titles[i+1];
            // }
            // doc.SaveToFile(coverPath, Spire.Doc.FileFormat.Doc);

        }

        public void CreateCover(string archPath)
        {

 
        }

        public void PrintCover(string coverPath)
        {


            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = "HP LaserJet Pro M701a UPD PCL 6";
            printDocument.PrintPage += PrintDocument_PrintPage;
            /*            //初始化Document实例
                        Document doc = new Document();

                        //加载一个Word文档
                        doc.LoadFromFile(coverPath);
                        doc.p
                        doc.SaveToFile(coverPath.Replace(".doc", ".pdf"), Spire.Doc.FileFormat.PDF);
            */
            /*            Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument();
                        pdf.LoadFromFile(coverPath.Replace(".doc", ".pdf"));
                        //使用默认打印机打印文档所有页面
                        pdf.Print();*/
            //获取PrintDocument对象
            /*            PrintDocument printDoc = doc.PrintDocument;
                        printDoc.PrinterSettings.PrinterName = "HP LaserJet Pro M701a UPD PCL 6";
                        printDoc.PrintPage += PrintDoc_PrintPage;*/
            /*    printDoc.PrinterSettings.DefaultPageSettings.Margins.Left = 133;
                printDoc.PrinterSettings.DefaultPageSettings.Margins.Bottom = 196;
                printDoc.PrinterSettings.DefaultPageSettings.Margins.Right = 133;
                printDoc.PrinterSettings.DefaultPageSettings.Margins.Top = 118; */
            //printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 210, 297);
            //设置PrintController属性为StandardPrintController，用于隐藏打印进程
            /*            printDoc.PrintController = new StandardPrintController();

                        //打印文档
                        printDoc.Print();*/
            /*
                        var newFile2 = @"newbook.core.docx";
                        using (var fs = new FileStream(newFile2, FileMode.Create, FileAccess.Write))
                        {
                            XWPFDocument doc = new XWPFDocument();
                            var p0 = doc.CreateParagraph();
                            doc.
                            p0.Alignment = ParagraphAlignment.CENTER;
                            XWPFRun r0 = p0.CreateRun();
                            r0.FontFamily = "microsoft yahei";
                            r0.FontSize = 18;
                            r0.IsBold = true;
                            r0.SetText("This is title");

                            var p1 = doc.CreateParagraph();
                            p1.Alignment = ParagraphAlignment.LEFT;
                            p1.IndentationFirstLine = 500;
                            XWPFRun r1 = p1.CreateRun();
                            r1.FontFamily = "·ÂËÎ";
                            r1.FontSize = 12;
                            r1.IsBold = true;
                            r1.SetText("This is content, content content content content content content content content content");

                            doc.Write(fs);
                        }*/

        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            ;
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            ;
        }

        public TitleToken BuildTitleToken(int width, int porjectNumber, int[] id, string projectName, string title)
        {
            TitleToken token = new TitleToken();
            // width
            token.Width = width;

            // Number
            token.Number = $"{projectName}·Y-{id.Min()}~{id.Max()}";
            token.Titles = new string[8];

            // 标题
            if (  projectName.Length > 15)
            {
                token.ProjecName = projectName.Substring(0, 15);
                token.Titles[0] += projectName.Substring(15);
            }
            else
            {
                token.ProjecName = projectName;
            }

            // title
            token.Titles[0] += title;
            for (int i = 0; i < 7 && token.Titles[i].Length > 0; i++)
            {
                if( token.Titles[i].Length > 15)
                {
                    token.Titles[i + 1] = token.Titles[i].Substring(15);
                    token.Titles[i] = token.Titles[i].Substring(0, 15);
                }
            }
            return token;
        }
        public void BuildTitle(string titlePath, TitleToken[] tokens, string demoName)
        {
            string demoPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
          
            demoPath = Path.Combine(demoPath, "Template", demoName);
            
            using (FileStream fs = new FileStream(demoPath, FileMode.Open, FileAccess.Read))
            {
                XWPFDocument doc = new XWPFDocument(fs);
                IList<XWPFParagraph> paragraphs = doc.Paragraphs;
                // Numbers
                XWPFParagraph[] ns = paragraphs.Where(p => p.Text.Contains("$Number$")).ToArray();
                // Project
                XWPFParagraph[] ps = paragraphs.Where(p => p.Text.Contains("$ProjectName$")).ToArray();
                // Titles
                XWPFParagraph[][] ts = new XWPFParagraph[8][];
                for (int i = 0; i < 8; i++)
                {
                    ts[i] = paragraphs.Where(p => p.Text.Contains($"$Title{i}$")).ToArray();
                }

                // tokens 
                for (int i = 0; i < tokens.Length; i++)
                {
                    ns[i].ReplaceText("$Number$", tokens[i].Number);
                    ps[i].ReplaceText("$ProjectName$", tokens[i].ProjecName);
                    for (int j = 0; j < tokens[i].Titles.Length; j++)
                    {
                        ts[i][j].ReplaceText($"$Title{i}$", tokens[i].Titles[j]);

                    }
                }

                FileStream dst = new FileStream(titlePath, FileMode.CreateNew, FileAccess.ReadWrite);
                doc.Write(dst);
                dst.Close();
                doc.Close();
            }

            // 加载模板


        }

        public void BuildCatalog(string catalogPath)
        {
            throw new System.NotImplementedException();
        }

        public Catalog[] ReadCatalog(string catalogPath)
        {
            List<Catalog> catalogs = new List<Catalog>();
            /*            using (FileStream fs = new FileStream(catalogPath, FileMode.Open, FileAccess.Read))
                        {
                            XWPFDocument doc = new XWPFDocument(fs);
                            IList<XWPFParagraph> paragraphs = doc.Paragraphs;
                            foreach (var row in doc.Tables[0].Rows)
                            {
                                Catalog catalog = new Catalog();
                                catalog.Id = row.GetCell(0).Paragraphs.First().Text;
                                catalog.Title = row.GetCell(1).Paragraphs.First().Text;
                                catalog.Author = row.GetCell(2).Paragraphs.First().Text;
                                catalog.Date = row.GetCell(3).Paragraphs.First().Text;
                                catalog.Number = row.GetCell(4).Paragraphs.First().Text;
                                catalog.PageNumber = row.GetCell(5).Paragraphs.First().Text;
                                catalog.Other = row.GetCell(6).Paragraphs.First().Text;
                                catalogs.Add(catalog);
                            }

                            doc.Close();
                        }*/
           

            Aspose.Words.Document doc = new Aspose.Words.Document(catalogPath);
            Aspose.Words.Tables.Table table = doc.GetChildNodes(NodeType.Table, true)[0] as Aspose.Words.Tables.Table;
            try
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Catalog catalog = new Catalog();
                    catalog.Id = table.Rows[i].Cells[0].ToString(SaveFormat.Text).Trim();
                    catalog.Title = table.Rows[i].Cells[1].ToString(SaveFormat.Text).Trim();
                    catalog.Author = table.Rows[i].Cells[2].ToString(SaveFormat.Text).Trim();
                    catalog.Date = table.Rows[i].Cells[3].ToString(SaveFormat.Text).Trim();
                    catalog.Number = table.Rows[i].Cells[4].ToString(SaveFormat.Text).Trim();
                    catalog.PageNumber = table.Rows[i].Cells[5].ToString(SaveFormat.Text).Trim();
                    catalog.Other = table.Rows[i].Cells[6].ToString(SaveFormat.Text).Trim();
                    catalogs.Add(catalog);
                }
            }
            catch
            {
                catalogs.Add(new Catalog());
            }

            /*foreach (Aspose.Words.Tables.Row row in table.Rows)
            {
                Catalog catalog = new Catalog();
                catalog.Id = row.Cells[0].ToString(SaveFormat.Text).Trim();
                catalog.Title = row.Cells[1].ToString(SaveFormat.Text).Trim();
                catalog.Author = row.Cells[2].ToString(SaveFormat.Text).Trim();
                //catalog.Date = row.Cells[3].ToString(SaveFormat.Text).Trim();
                catalog.Number = row.Cells[4].ToString(SaveFormat.Text).Trim();
                catalog.PageNumber = row.Cells[5].ToString(SaveFormat.Text).Trim();
                catalog.Other = row.Cells[6].ToString(SaveFormat.Text).Trim();
                catalogs.Add(catalog);
            }*/
            return catalogs.ToArray();
        }
    }
}
