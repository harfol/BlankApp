using BlankApp.Service;
using BlankApp.Service.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlankApp.ExcelAddIn
{

    public partial class Form1 : Form, INotifyPropertyChanged
    {
        #region 属性
        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; NotifyPropertyChanged("Code"); }
        }
        private string _cmd;
        public string Cmd
        {
            get { return _cmd; }
            set { _cmd = value; NotifyPropertyChanged("Cmd"); }
        }
        private string _msg;
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; NotifyPropertyChanged("Msg"); }
        }
        private string _tempFile;
        public string TempFile
        {
            get { return _tempFile; }
            set { _tempFile = value; NotifyPropertyChanged("TempFile"); }
        }
        private string _workingDirectory;
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; NotifyPropertyChanged("WorkingDirectory"); }
        }
        public Dictionary<string, List<string>> SqlModels { get; set; }
        public Dictionary<string, List<string>> TempModels { get; set; }

        private IConfigurationService _cs;
        #endregion

        #region 绑定
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        public bool IsClose { get; set; } = true;
        public Form1()
        {
            InitializeComponent();
        }

        public Form1(Dictionary<string, List<string>> o)
        {
            _cs = new ConfigurationService();
            SqlModels = o;
            TempModels = new Dictionary<string, List<string>>();
            
            this.Cmd = _cs.AppSettings["DefaultCmd"];
            this.TempFile = _cs.AppSettings["DefaultTempFile"];
            this.WorkingDirectory = _cs.AppSettings["DefaultWorkingDirectory"];
            InitializeComponent();
            
            ShowSQL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtCmd.DataBindings.Add("Text", this, "Cmd");
            this.txtCode.DataBindings.Add("Text", this, "Code");
            this.txtMsg.DataBindings.Add("Text", this, "Msg");
            this.txtTempFile.DataBindings.Add("Text", this, "TempFile");
            this.txtWorkingDirectory.DataBindings.Add("Text", this, "WorkingDirectory");
            this.FormClosed += (s, args) => this.IsClose = true;
            // 加选项
            foreach (var item in SqlModels)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Text = item.Key;
                checkBox.Checked = true;
                checkBox.AutoSize = true;
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                this.flp.Controls.Add(checkBox);
            }
        }
        public void ShowSQL()
        {
            /* 做索引的字段要放在 SqlModels 的第一个。 */
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE baiyun_project_archives_copy");
            sb.Append("  SET");
            for (int i = 1; i < SqlModels.Count; i++)
            {
                sb.AppendLine($" {SqlModels.ElementAt(i).Key} = CASE {SqlModels.ElementAt(0).Key}");
                for (int j = 0; j < SqlModels.ElementAt(0).Value.Count; j++)
                {
                    sb.AppendLine($"    WHEN '{SqlModels.ElementAt(0).Value[j]}' THEN '{SqlModels.ElementAt(i).Value[j]}'");
                }
                sb.Append("  END,");
            }

            sb.Remove(sb.Length - 1, 1);    // 去掉最后的','
            sb.AppendLine($" WHERE {SqlModels.ElementAt(0).Key} IN (");
            foreach (var item in SqlModels.ElementAt(0).Value)
            {
                sb.AppendLine($"  '{item}',");
            }
            
            sb.Remove(sb.Length - Environment.NewLine.Length - 1, 1);    // 去掉最后的','
            sb.AppendLine(");");
            this.Code = sb.ToString();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            /* 也就是选中的放在SqlModels，没选中的放在TempModels而已。*/
            CheckBox checkBox = sender as CheckBox;
            if( checkBox.Checked && !SqlModels.ContainsKey(checkBox.Text) && TempModels.ContainsKey(checkBox.Text))
            {
                SqlModels.Add(checkBox.Text,  TempModels[checkBox.Text]);
                TempModels.Remove(checkBox.Text);
            }
            else if( !checkBox.Checked && !TempModels.ContainsKey(checkBox.Text) && SqlModels.ContainsKey(checkBox.Text))
            {
                TempModels.Add(checkBox.Text, SqlModels[checkBox.Text]);
                SqlModels.Remove(checkBox.Text);
            }
            ShowSQL();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            File.WriteAllText(TempFile, this.Code);
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = "cmd.exe";
            pInfo.WorkingDirectory = WorkingDirectory;
            pInfo.CreateNoWindow = true;
            pInfo.UseShellExecute = false;
            pInfo.RedirectStandardInput = true;
            pInfo.RedirectStandardOutput = true;
            pInfo.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = pInfo;
            process.Start();
            process.StandardInput.WriteLine($"{this.Cmd} {TempFile} &exit");
            process.StandardInput.AutoFlush = true;

            this.Msg = "[UPDATE]:\r\n";
            foreach (var item in SqlModels.First().Value)
            {
                this.Msg += "    "+item + "\r\n\r\n";
            } 
            this.Msg += process.StandardOutput.ReadToEnd();
            this.Msg += process.StandardError.ReadToEnd();
        }
    }
}
