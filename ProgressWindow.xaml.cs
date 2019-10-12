using MahApps.Metro.Controls;
using System;
using System.ComponentModel;

namespace RetroFilter
{
    public partial class ProgressWindow : MetroWindow
    {
        public ProgressWindow()
        {
            //Owner = window;
            InitializeComponent();
            Hide();
        }

        public void Show(Action<IProgress<string>> work)
        {
            BackgroundWorker worker = new BackgroundWorker();
            Progress<string> progress = new Progress<string>(data => txtInfo.Text = data);

            worker.DoWork += (s, workerArgs) => work(progress);
            worker.RunWorkerCompleted +=(s, workerArgs) => { Hide(); };
            worker.RunWorkerAsync();

            ShowDialog();
        }
    }
}
