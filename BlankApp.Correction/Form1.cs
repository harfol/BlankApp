using BlankApp.Service;
using BlankApp.Service.Impl;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;

namespace BlankApp.Correction
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private IConfigurationService _cs;
        private string correction(string str)
        {
            string trim = str.Replace(" ", "");
            foreach (string key in _cs.CorrectionSettings.Keys)
            {
                trim = trim.Replace(key, _cs.CorrectionSettings[key]);
            }
            return trim;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _cs = new ConfigurationService();
        }

        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Escape)
            {
                this.txt.Text = correction(this.txt.Text);
                this.txt.Select(this.txt.TextLength, 0);
                this.txt.ScrollToCaret();

            }
            
        }

        private void txt_MouseUp(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Right)
            {
                int tota = txt.GetLineFromCharIndex(txt.MaxLength) + 1;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.gb.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
