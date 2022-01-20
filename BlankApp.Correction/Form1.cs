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
        NameValueCollection kvs;

        private string correction(string str)
        {
            string trim = str.Replace(" ", "");
            foreach (string key in kvs.AllKeys)
            {
                trim = trim.Replace(key, kvs[key]);
            }
            return trim;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            kvs = ConfigurationManager.GetSection("correction") as NameValueCollection;
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
    }
}
