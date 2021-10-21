using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlankApp.Views
{
    /// <summary>
    /// ProgressBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBarWindow : Window
    {

        

        private BackgroundWorker BackgroundWorker { get; set; }
        private Action<BackgroundWorker, int> Function;

        public ProgressBarWindow()
        {
            InitializeComponent();

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            this.BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            this.BackgroundWorker.WorkerReportsProgress = true;
            this.BackgroundWorker.WorkerSupportsCancellation = true;
            this.ProgressBar.Value = 0;
            this.ProgressBar.Maximum = 100;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double result = (double)e.ProgressPercentage / this.ProgressBar.Maximum;
            this.ProgressBar.DataContext = result;
            this.ProgressBar.Value = (double)e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            this.Function(worker, i);
        }
        public void Show(string title, double maximum, Action<BackgroundWorker, int> func)
        {
            this.ProgressBar.Tag = title;
            this.ProgressBar.Maximum = maximum;
            this.Function = func;

            this.BackgroundWorker.RunWorkerAsync();
            this.ShowDialog();

        }
    }
}
