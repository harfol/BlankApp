using Aspose.Words;
using BlankApp.Service.Extensions;
using BlankApp.Service.Model;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;

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
            string project = "";// projectName.Substring(0, projectName.IndexOf("项目"));
            /*
            if (project.Length <= 18)
            {
                project = (project + "项目").PadRightEx(40, ' ');
            }
            else
            {
                project = project.Substring(0, 19);
                other = project.Substring(19) + "项目";
                project = project.PadRightEx(40, ' ');
            }*/
            if( projectName.Length > 20)
            {
                project = projectName.Substring(0, 19).PadRightEx(40, ' ');
                other = projectName.Substring(19);
            }
            else
            {
                project = projectName.PadRightEx(40, ' ');
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



        private void SetTableBorderStyle(Microsoft.Office.Interop.Word.Table table1, 
            Microsoft.Office.Interop.Word.WdBorderType wdBorderLeft, 
            Microsoft.Office.Interop.Word.WdColor wdColorGray50,
            Microsoft.Office.Interop.Word.WdLineWidth wdLineWidth050pt)
        {
            table1.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderLeft].Visible = true;
            table1.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderLeft].Color = Microsoft.Office.Interop.Word.WdColor.wdColorGreen;

            table1.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderLeft].LineWidth = Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt;
        }

        public void BuildCatalog(string catalogPath, string dossier, Catalog[] catalogs)
        {
            BuildCatalog(catalogPath, dossier, catalogs, null);
        }



        public void BuildCatalog(string catalogPath, string dossier, Catalog[] catalogs, Action<string> action)
        {
            string saveFilePath = "";

            try
            {

                action?.Invoke("初始化文档");
                object path = catalogPath;//文件路径
                Microsoft.Office.Interop.Word.Application wordApp;//Word应用程序变量
                Microsoft.Office.Interop.Word.Document wordDoc;//Word文档变量

                wordApp = new Microsoft.Office.Interop.Word.Application();//初始化
                if (File.Exists((string)path))
                {
                    action?.Invoke("文档已存在，删除 "+path);
                    File.Delete((string)path);
                }

                //由于使用的是COM库，因此有许多变量需要用Missing.Value代替
                Object Nothing = Missing.Value;
                wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);

                action?.Invoke("设置页面");
                #region 页面设置

                //页面设置
                wordDoc.PageSetup.PaperSize = Microsoft.Office.Interop.Word.WdPaperSize.wdPaperA4;//设置纸张样式为A4纸
                wordDoc.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientPortrait;//排列方式为垂直方向
                wordDoc.PageSetup.TopMargin = 57.0f;
                wordDoc.PageSetup.BottomMargin = 57.0f;
                wordDoc.PageSetup.LeftMargin = 57.0f;
                wordDoc.PageSetup.RightMargin = 57.0f;
                #endregion

                #region 页码设置并添加页码

                //为当前页添加页码
                /*MSWord.PageNumbers pns = wordApp.Selection.Sections[1].Headers[MSWord.WdHeaderFooterIndex.wdHeaderFooterEvenPages].PageNumbers;//获取当前页的号码
                pns.NumberStyle = MSWord.WdPageNumberStyle.wdPageNumberStyleNumberInDash;//设置页码的风格，是Dash形还是圆形的
                pns.HeadingLevelForChapter = 0;
                pns.IncludeChapterNumber = false;
                pns.RestartNumberingAtSection = false;
                pns.StartingNumber = 0; //开始页页码？
                object pagenmbetal = MSWord.WdPageNumberAlignment.wdAlignPageNumberCenter;//将号码设置在中间
                object first = true;
                wordApp.Selection.Sections[1].Footers[MSWord.WdHeaderFooterIndex.wdHeaderFooterEvenPages].PageNumbers.Add(ref pagenmbetal, ref first);*/

                #endregion

                action?.Invoke("添加标题");
                #region 卷内征地文件材料目录
                wordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                wordDoc.Paragraphs.Last.Range.Text = "卷内征地文件材料目录";
                wordDoc.Paragraphs.Last.Range.Font.Bold = 1;
                wordDoc.Paragraphs.Last.Range.Font.Size = 18;
                wordDoc.Paragraphs.Add(ref Nothing);
                wordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                #endregion

                action?.Invoke("添加档案号");
                #region 档案号
                wordDoc.Paragraphs.Last.Range.Text = $"档案号\t{dossier}";
                wordDoc.Paragraphs.Last.Range.Font.Bold = 0;
                wordDoc.Paragraphs.Last.Range.Font.Size = 12;
                wordDoc.Paragraphs.Add(ref Nothing);

                object unite = Microsoft.Office.Interop.Word.WdUnits.wdStory;
                #endregion

                #region 添加表格
                //表格
                wordDoc.Content.InsertAfter("\n");//这一句与下一句的顺序不能颠倒
                wordApp.Selection.EndKey(ref unite, ref Nothing); //将光标移动到文档末尾
                wordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                int tableRow = catalogs.Length+1;
                int tableColumn = 7;
                List<string> detailList = new List<string>();
                //定义一个word中的表格对象
                Microsoft.Office.Interop.Word.Table table = wordDoc.Tables.Add(wordApp.Selection.Range, tableRow, tableColumn, ref Nothing, ref Nothing);

                //列宽
                //table.PreferredWidth = 400f;
                table.Columns[1].Width = 30f;
                table.Columns[2].Width = 155f;
                table.Columns[3].Width = 60f;
                table.Columns[4].Width = 50f;
                table.Columns[5].Width = 100f;
                table.Columns[6].Width = 50f;
                table.Columns[7].Width = 50f;


                wordDoc.Tables[1].Cell(1, 1).Range.Text = "顺\n序\n号";
                wordDoc.Tables[1].Cell(1, 2).Range.Text = "征地文件材料标题";
                wordDoc.Tables[1].Cell(1, 3).Range.Text = "文件作者";
                wordDoc.Tables[1].Cell(1, 4).Range.Text = "成文\n日期";
                wordDoc.Tables[1].Cell(1, 5).Range.Text = "文件编号";
                wordDoc.Tables[1].Cell(1, 6).Range.Text = "文件\n页码";
                wordDoc.Tables[1].Cell(1, 7).Range.Text = "备注";
                #endregion

                
                for (int i = 0; i < catalogs.Length; i++)
                {
                    wordDoc.Tables[1].Cell(2 + i, 1).Range.Text = catalogs[i].Id;
                    wordDoc.Tables[1].Cell(2 + i, 2).Range.Text = catalogs[i].Title;
                    wordDoc.Tables[1].Cell(2 + i, 3).Range.Text = catalogs[i].Author;
                    wordDoc.Tables[1].Cell(2 + i, 4).Range.Text = catalogs[i].Date;
                    wordDoc.Tables[1].Cell(2 + i, 5).Range.Text = catalogs[i].Number;
                    wordDoc.Tables[1].Cell(2 + i, 6).Range.Text = catalogs[i].PageNumber;
                    wordDoc.Tables[1].Cell(2 + i, 7).Range.Text = catalogs[i].Other;

                }

                #region 设置表格样式
                //设置table样式
                table.Rows.HeightRule = Microsoft.Office.Interop.Word.WdRowHeightRule.wdRowHeightAtLeast;
                table.Rows.Height = wordApp.CentimetersToPoints(float.Parse("0.8"));

                table.Range.Font.Size = 10.5F;
                table.Range.Font.Bold = 0;

                table.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;//表格文本居中
                table.Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;//文本垂直居中

                for (int a = 2; a <= table.Rows.Count; a++)
                {
                    table.Rows[a].Cells[2].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;  //标题文本左对齐
                }

                //设置table边框样式
                table.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;//表格外框是单线
                table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;//表格内框是单线

                //设置table边框样式
                table.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                table.Rows[1].Height = 60;          //标题高度

                //表格边框
                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderHorizontal, Microsoft.Office.Interop.Word.WdColor.wdColorGray20, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth025pt);

                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderVertical, Microsoft.Office.Interop.Word.WdColor.wdColorGray20, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth025pt);

                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderLeft, Microsoft.Office.Interop.Word.WdColor.wdColorGray50, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt);

                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderRight, Microsoft.Office.Interop.Word.WdColor.wdColorGray50, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt);

                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderTop, Microsoft.Office.Interop.Word.WdColor.wdColorGray50, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt);

                SetTableBorderStyle(table, Microsoft.Office.Interop.Word.WdBorderType.wdBorderBottom, Microsoft.Office.Interop.Word.WdColor.wdColorGray50, Microsoft.Office.Interop.Word.WdLineWidth.wdLineWidth050pt);
                #endregion

                action?.Invoke($"保存 {catalogPath}");
                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;
                wordDoc.SaveAs(ref path, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
                wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
                saveFilePath += path;

            }
            catch (Exception e)
            {
                Console.WriteLine("Word文档创建失败,原因:" + e.Message);
            }

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
