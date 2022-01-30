using BlankApp.Service;
using BlankApp.Service.Impl;
using BlankApp.Service.Model;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlankApp.Input
{

    public partial class Form : System.Windows.Forms.Form
    {
        public class PathMsg
        {
            public int ProNumber { get; set; }
            public int ArchNumber { get; set; }
            public string SerialNumber { get; set; }
            public string WorkPath { get; set; }
            public string ParentPath { get; set; }

            public static PathMsg GetPathMsg(string path)
            {
                if (string.IsNullOrWhiteSpace(path)) return null;

                PathMsg pathMsg = new PathMsg();

                string archName = Path.GetFileName(path);
                string proName = Path.GetFileName(Path.GetDirectoryName(path));
                int num;
                if (archName.Length > 4 && int.TryParse(archName.Substring(0, 4), out num))
                {
                    pathMsg.ArchNumber = num;
                }
                if (proName.Length >= 5
                    && proName.Substring(0, 2).Equals("ZY", StringComparison.InvariantCultureIgnoreCase)
                    && int.TryParse(proName.Substring(2, 3), out num))
                {
                    pathMsg.ProNumber = num;
                }


                if (pathMsg.ProNumber != 0 && pathMsg.ArchNumber != 0)
                {
                    pathMsg.SerialNumber = $"ZY{pathMsg.ProNumber:D3}{pathMsg.ArchNumber:D4}";
                }

                pathMsg.WorkPath = path;
                pathMsg.ParentPath = Path.GetDirectoryName(path);

                return pathMsg;
            }
        }
        public class ArticleMsg : Detail
        {
            private int _startPage = 0;
            public int StartPage
            {
                get { return _startPage; }
                set
                {
                    SetProperty(ref _startPage, value);
                    this.PageNumber = $"{this._startPage}" + ((_startPage != _endPage && _endPage != 0) ? $"-{_endPage}" : "");
                }
            }

            private int _endPage = 0;
            public int EndPage
            {
                get { return _endPage; }
                set
                {
                    SetProperty(ref _endPage, value);
                    this.PageNumber = $"{this._startPage}" + ((_startPage != _endPage && _endPage != 0) ? $"-{_endPage}" : "");
                }
            }

            private bool _isNotBuild;
            public bool IsNotBuild
            {
                get { return _isNotBuild; }
                set { SetProperty(ref _isNotBuild, value); }
            }

            private Color _foreColor;
            public Color ForeColor
            {
                get { return _foreColor; }
                set { SetProperty(ref _foreColor, value); }
            }

            public ArticleToken ArticleToken { get; set; }
            public string Pdf { get; set; }
            public string Txt { get; set; }
            public string Dir { get; set; }



        }

        private readonly IHistoryService _historyService;

        public ArchiveInfo ArchiveInfo { get; set; } = new ArchiveInfo();
        public string BinName { get; private set; }

        public PathMsg CurrentPathMsg { get; set; }

        public Form(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    ConfigurationService cs = new ConfigurationService();
                    _historyService = new HistoryService(cs);
                    string path = args[0].Equals(".") ? Path.GetFullPath(args[0]) : args[0];

                    CurrentPathMsg = PathMsg.GetPathMsg(path);

                    if (!string.IsNullOrEmpty(CurrentPathMsg.SerialNumber))
                    {
                        int num = GetLastFileNo();
                        ArchiveInfo.No = (num + 1).ToString("D2");
                        ArchiveInfo.Page = "1";
                    }

                    BinName = cs.AppSettings["BlankAppCliBinName"] + " ";

                    InitializeComponent();
                    ListOFF();
                }
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "input_log.txt"), e.ToString());
            }

        }

        private void ListON()
        {
            this.gbList.Visible = true;
            this.Size = new Size(1220, 590);
            this.ckbList.Checked = true;
        }
        private void ListOFF()
        {
            this.gbList.Visible = false;
            this.Size = new Size(1220, 190);
            this.ckbList.Checked = false;
        }
        private int GetLastFileNo()
        {
            string[] paths = Directory.GetDirectories(CurrentPathMsg.WorkPath, "*", SearchOption.TopDirectoryOnly);
            if (paths.Length > 0)
            {
                string ns = Path.GetFileName(paths.Last()).Substring(0, 2);
                return int.Parse(ns);
            }
            return 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            this.cbTitle1.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cbTitle1.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cbTitle1.DataSource = _historyService.List(1);
            this.cbTitle1.DataBindings.Add("Text", ArchiveInfo, "Title1");

            this.cbTitle2.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cbTitle2.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cbTitle2.DataSource = _historyService.List(2);
            this.cbTitle2.DataBindings.Add("Text", ArchiveInfo, "Title2");

            this.cbTitle3.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cbTitle3.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cbTitle3.DataSource = _historyService.List(3);
            this.cbTitle3.DataBindings.Add("Text", ArchiveInfo, "Title3");


            this.txtNo.DataBindings.Add("Text", ArchiveInfo, "No");
            this.txtPage.DataBindings.Add("Text", ArchiveInfo, "Page");

            this.Text = CurrentPathMsg.WorkPath;

            if (!string.IsNullOrEmpty(CurrentPathMsg.SerialNumber))
            {
                this.txtNumber.Text = CurrentPathMsg.SerialNumber;
            }
            ReFlashLabel();

            this.lypList.Controls.Clear();
        }

        #region 基础功能

        private void ReFlashLabel()
        {
            // 标题显示
            this.lbLabel.Text = $"案卷号：ZY·ZD·{CurrentPathMsg.ProNumber}·Y-{CurrentPathMsg.ArchNumber:D3}     "
                              + $"测量编号：{this.txtMesure.Text}    "
                              + $"权属人：{this.txtOwner.Text}";
        }

        public void Prev()
        {

            if (!string.IsNullOrEmpty(CurrentPathMsg.SerialNumber) && CurrentPathMsg.ArchNumber > 1)
            {
                int an = CurrentPathMsg.ArchNumber - 1;
                string prev = Directory.GetDirectories(CurrentPathMsg.ParentPath, an.ToString("D4") + "*").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(prev))
                {
                    CurrentPathMsg = PathMsg.GetPathMsg(prev);
                    this.txtNumber.Text = CurrentPathMsg.SerialNumber;
                    this.Text = CurrentPathMsg.WorkPath;
                    ArchiveInfo.No = (GetLastFileNo() + 1).ToString("D2");
                    ArchiveInfo.Page = "1";
                    this.txtMesure.Text = "";
                    this.txtOwner.Text = "";
                    this.lypList.Controls.Clear();
                    ReFlashLabel();
                }
            }
        }

        public void Next()
        {
            if (!string.IsNullOrEmpty(CurrentPathMsg.SerialNumber))
            {
                int an = CurrentPathMsg.ArchNumber + 1;
                string next = Directory.GetDirectories(CurrentPathMsg.ParentPath, an.ToString("D4") + "*").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(next))
                {
                    CurrentPathMsg = PathMsg.GetPathMsg(next);
                    this.txtNumber.Text = CurrentPathMsg.SerialNumber;
                    this.Text = CurrentPathMsg.WorkPath;
                    ArchiveInfo.No = (GetLastFileNo() + 1).ToString("D2");
                    ArchiveInfo.Page = "1";
                    this.txtMesure.Text = "";
                    this.txtOwner.Text = "";
                    this.lypList.Controls.Clear();
                    ReFlashLabel();
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.cbTitle3.Text))
            {
                string cmd = $"创建条目 {this.CurrentPathMsg.WorkPath} {ArchiveInfo}";
                Process.Start("cmd", "/C" + BinName + cmd);
                this.ArchiveInfo.No = (int.Parse(this.ArchiveInfo.No) + 1).ToString("D2");
                this.ArchiveInfo.Page = "1";
            }
        }

        private void btnTxtBuildSimple_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtOwner.Text))
            {

                string cmd = $"根据文件夹名称创建TXT文件 {CurrentPathMsg.WorkPath} \"{txtMesure.Text}\" {txtOwner.Text} {txtNumber.Text} {txtYear.Text}";
                Process.Start("cmd", "/C" + BinName + cmd);
            }
        }
        private void btnReadAgain_Click(object sender, EventArgs e)
        {
            this.cbTitle1.DataSource = null;
            this.cbTitle2.DataSource = null;
            this.cbTitle3.DataSource = null;
            _historyService.Refresh();
            this.cbTitle1.DataSource = _historyService.List(1);
            this.cbTitle2.DataSource = _historyService.List(2);
            this.cbTitle3.DataSource = _historyService.List(3);
            this.cbTitle1.Text = "";
            this.cbTitle2.Text = "";
            this.cbTitle3.Text = "";
        }

        private void btnBuildCover_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"创建封面 {CurrentPathMsg.WorkPath}");
        }

        private void btnBuildCatalog_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"创建目录 {CurrentPathMsg.WorkPath}");
        }

        private void bthCopyPDF_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"按照文件夹名称拷贝PDF {CurrentPathMsg.WorkPath}");
        }

        private void btnCopyCover_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"拷贝封面 {CurrentPathMsg.WorkPath} \"{txtMask.Text}\"");
        }

        private void btnCopyCatalog_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"拷贝目录 {CurrentPathMsg.WorkPath} \"{txtMask.Text}\"");
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            Process.Start("cmd", "/C" + BinName + $"统计 {CurrentPathMsg.WorkPath} \"{txtMask.Text}\"");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Next();

        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            Prev();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {


            string[] dirs = Directory.GetDirectories(CurrentPathMsg.WorkPath, "*", SearchOption.TopDirectoryOnly);
            string no = this.txtNo.Text;
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
            Directory.CreateDirectory(Path.Combine(CurrentPathMsg.WorkPath, ArchiveInfo.ToString()));
            this.txtNo.Text = (int.Parse(no) + 1).ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string[] dirs = Directory.GetDirectories(CurrentPathMsg.WorkPath, "*", SearchOption.TopDirectoryOnly);
            string no = this.txtNo.Text;
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
        private void ckbDanger_Click(object sender, EventArgs e)
        {
            this.gpbDanger.Enabled = this.ckbDanger.Checked;
        }



        #endregion

        private void btnTxtBuild_Click(object sender, EventArgs e)
        {
            ArticleServiceBase articleServiceBase = new RawArticleService();

            foreach (ArticleMsg arti in Temp)
            {
                try
                {
                    string txt = Path.Combine(arti.Dir, arti.ArticleToken.Name + ".txt");
                    string pdf = Path.Combine(arti.Dir, arti.ArticleToken.Name + ".pdf");
                    articleServiceBase.WriteTxtProperties(txt, arti);
                    File.Move(arti.Pdf, pdf);
                    arti.Pdf = pdf;
                    arti.Txt = txt;
                    arti.ForeColor = Color.LimeGreen;
                    // arti.IsNotBuild = false;
                }
                catch (Exception ex)
                {
                    // 移动文件可能会报错
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int id = int.Parse(((sender as Button).Parent.Controls[0] as TextBox).Text);
            int endPageNumber = Temp[id - 1].EndPage;
            for (int i = id; i < Temp.Length; i++)
            {
                int page = int.Parse(Temp[i].Pages);
                int start = endPageNumber + 1;
                int end = endPageNumber + page;
                Temp[i].StartPage = start;
                Temp[i].EndPage = end;
                endPageNumber += page;
            }
        }

        public ArticleMsg[] Temp { get; set; }
        private void btnReFlashList_Click(object sender, EventArgs e)
        {
            if (CurrentPathMsg.ArchNumber <= 0) return;

            // 进度条
            this.pbLoad.Minimum = 0;
            int size = Directory.GetDirectories(CurrentPathMsg.WorkPath, "*", SearchOption.TopDirectoryOnly).Length;
            this.pbLoad.Maximum = size * 2;

            ReFlashLabel();

            Temp = null;
            ArticleMsg[] details = null;

            #region   搜索文件夹，初始化details
            ArticleServiceBase articleServiceBase = new RawArticleService();
            string[] dirs = Directory.GetDirectories(CurrentPathMsg.WorkPath, "*", SearchOption.TopDirectoryOnly);
            Array.Sort<string>(dirs);
            details = new ArticleMsg[dirs.Length];
            #endregion

            #region  初始化details填入信息
            int pageNumberStart = 0;    // 开始页码
            for (int i = 0; i < dirs.Length; i++)
            {
                ArticleToken at = articleServiceBase.GetArticleToken(dirs[i]);
                string pdf = Directory.GetFiles(CurrentPathMsg.WorkPath, $"{at.Id}.pdf", SearchOption.TopDirectoryOnly).FirstOrDefault();
                int page = 0;    // 页数
                using (PdfReader pdfReader = new PdfReader(pdf))
                {
                    page = pdfReader.NumberOfPages;
                }
                details[i] = new ArticleMsg();
                details[i].Title = at.Name;
                details[i].Copies = at.Copies;
                details[i].Pages = page.ToString();
                details[i].Measure = this.txtMesure.Text;
                details[i].Owner = this.txtOwner.Text;
                details[i].Year = this.txtYear.Text;
                details[i].Number = this.txtNumber.Text;
                int start = pageNumberStart + 1;
                int end = pageNumberStart + page;
                details[i].StartPage = start;
                details[i].EndPage = end;
                pageNumberStart += page;

                details[i].ArticleToken = at;
                details[i].Pdf = pdf;
                details[i].Dir = dirs[i];
                //details[i].IsNotBuild = true;
                details[i].ForeColor = Color.Brown;
                this.pbLoad.PerformStep();
            }
            #endregion

            #region 生成列表ui

            this.lypList.Controls.Clear();    //清除旧的
            for (int i = 0; i < details.Length; i++)
            {
                // 
                // textBox1
                // 
                TextBox textBox1 = new TextBox();
                textBox1.Location = new System.Drawing.Point(3, 3);
                textBox1.Size = new System.Drawing.Size(30, 21);
                textBox1.TabIndex = 0;
                textBox1.TextAlign = HorizontalAlignment.Center;
                textBox1.Text = (i + 1).ToString("D2");
                //textBox1.DataBindings.Add("Enabled", details[i], "IsNotBuild");
                textBox1.DataBindings.Add("ForeColor", details[i], "ForeColor");

                // 
                // textBox2
                // 
                TextBox textBox2 = new TextBox();
                textBox2.Location = new System.Drawing.Point(39, 3);
                textBox2.Size = new System.Drawing.Size(500, 21);
                textBox2.TabIndex = 1;
                textBox2.DataBindings.Add("Text", details[i], "Title");
                // 
                // textBox3
                // 
                TextBox textBox3 = new TextBox();
                textBox3.Location = new System.Drawing.Point(545, 3);
                textBox3.Size = new System.Drawing.Size(320, 21);
                textBox3.TabIndex = 2;
                textBox3.DataBindings.Add("Text", details[i], "SubTitle");
                // 
                // textBox4
                // 
                TextBox textBox4 = new TextBox();
                textBox4.Location = new System.Drawing.Point(871, 3);
                textBox4.Size = new System.Drawing.Size(50, 21);
                textBox4.TabIndex = 3;
                textBox4.KeyUp += TextBox4_KeyUp;
                textBox4.DataBindings.Add("Text", details[i], "Year");
                // 
                // textBox5
                // 
                TextBox textBox5 = new TextBox();
                textBox5.Location = new System.Drawing.Point(927, 3);
                textBox5.Size = new System.Drawing.Size(40, 21);
                textBox5.TabIndex = 4;
                textBox5.DataBindings.Add("Text", details[i], "Copies");
                // 
                // textBox6
                // 
                TextBox textBox6 = new TextBox();
                textBox6.Location = new System.Drawing.Point(973, 3);
                textBox6.Size = new System.Drawing.Size(40, 21);
                textBox6.TabIndex = 5;
                textBox6.DataBindings.Add("Text", details[i], "Pages");
                // 
                // textBox7
                // 
                TextBox textBox7 = new TextBox();
                textBox7.Location = new System.Drawing.Point(1019, 3);
                textBox7.Size = new System.Drawing.Size(25, 21);
                textBox7.TabIndex = 6;
                textBox7.DataBindings.Add("Text", details[i], "StartPage");
                // 
                // button1
                // 
                Button button1 = new Button();
                button1.Location = new System.Drawing.Point(1050, 3);
                button1.Size = new System.Drawing.Size(15, 21);
                button1.Click += Button1_Click;
                button1.TabIndex = 7;
                button1.Text = ">";
                button1.UseVisualStyleBackColor = true;
                // 
                // textBox8
                // 
                TextBox textBox8 = new TextBox();
                textBox8.Location = new System.Drawing.Point(1071, 3);
                textBox8.Size = new System.Drawing.Size(25, 21);
                textBox8.TabIndex = 8;
                textBox8.DataBindings.Add("Text", details[i], "EndPage");
                // 
                // textBox9
                // 
                TextBox textBox9 = new TextBox();
                textBox9.Location = new System.Drawing.Point(1102, 3);
                textBox9.Name = "textBox9";
                textBox9.Size = new System.Drawing.Size(50, 21);
                textBox9.TabIndex = 9;
                textBox9.DataBindings.Add("Text", details[i], "Dossier");

                FlowLayoutPanel lypSub = new FlowLayoutPanel();
                lypSub.AutoSize = true;
                lypSub.Controls.Add(textBox1);
                lypSub.Controls.Add(textBox2);
                lypSub.Controls.Add(textBox3);
                lypSub.Controls.Add(textBox4);
                lypSub.Controls.Add(textBox5);
                lypSub.Controls.Add(textBox6);
                lypSub.Controls.Add(textBox7);
                lypSub.Controls.Add(button1);
                lypSub.Controls.Add(textBox8);
                lypSub.Controls.Add(textBox9);
                lypSub.TabIndex = i + 3;

                this.lypList.Controls.Add(lypSub);
                this.lypList.Controls.SetChildIndex(lypSub, i);
                this.pbLoad.PerformStep();
            }
            #endregion

            Temp = details;
        }

        private void TextBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                (sender as TextBox).Text = "";

            }
        }

        private void ckbList_Click(object sender, EventArgs e)
        {
            if (this.ckbList.Checked)
            {
                ListON();
            }
            else
            {
                ListOFF();
            }
        }

        private void txtPage_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnMergePDF_Click(object sender, EventArgs e)
        {
            string[] pdfs = this.txtMask.Text.Trim().Split(';');
            if (null != pdfs && pdfs.Length > 1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = CurrentPathMsg.WorkPath;
                saveFileDialog.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    Process.Start("cmd", "/C" + BinName + $"合并PDF{pdfs.Length-1} {this.txtMask.Text.Replace(';', ' ')}  {saveFileDialog.FileName}");
                    if( File.Exists(saveFileDialog.FileName))
                    {
                        this.txtMask.Text = "";
                    }
                }
            }
        }

        private void btnSelectPDF_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = CurrentPathMsg.WorkPath;
                openFileDialog.Filter = "pdf files (*.pdf)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] pdfs = openFileDialog.FileNames;
                    foreach (string pdf in pdfs)
                    {
                        string name = Path.GetFileName(pdf);
                        if (File.Exists(Path.Combine(CurrentPathMsg.WorkPath, name)) && pdf.EndsWith(".pdf"))
                        {
                            this.txtMask.Text += $"{pdf};";
                        }
                    }
                }
            }
        }
    }
}

