using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace RetroFilter
{
    public partial class ProcessWindow : MetroWindow
    {
        ItemCollection collection;
        DataFile.Type type;

        public ProcessWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public void Show(DataFile.Type t, ItemCollection c)
        {
            type = t;
            collection = c;
            btnSource.Content = "";
            btnDestination.Content = "";
            Show();
        }

        private void btnProcessSource_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
            {
                btnSource.Content = dialog.FileName;
            }
        }

        private void btnProcessDestination_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
            {
                btnDestination.Content = dialog.FileName;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string src_dir = btnSource.Content.ToString();
            string dst_dir = btnDestination.Content.ToString();

            if (collection.IsEmpty)
            {
                this.ShowMessageAsync("Ho No !", "List is empty, nothing to do...");
                return;
            }

            if (!Directory.Exists(src_dir) || !Directory.Exists(dst_dir)
                || string.IsNullOrEmpty(src_dir) || string.IsNullOrEmpty(dst_dir))
            {
                this.ShowMessageAsync("Ho No !", "Either source or destination directory is wrong...");
                return;
            }

            if (src_dir == dst_dir)
            {
                this.ShowMessageAsync("Ho No !", "Source directory can't be destination directory...");
                return;
            }

            src_dir += Path.DirectorySeparatorChar;
            dst_dir += Path.DirectorySeparatorChar;
            pbProgress.Maximum = collection.Count;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(new string[] { src_dir, dst_dir });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] dirs = (string[])e.Argument;
            List<string> missing = new List<string>();

            for (int i = 0; i < collection.Count; i++)
            {
                string name = type == DataFile.Type.EmulationStation ?
                    Path.GetFileName(((Game)collection[i]).Path) : ((Game)collection[i]).Name;
                if (name.Length < 4 || name[name.Length - 3] != '.')
                {
                    name += ".zip";
                }
                string src_path = dirs[0] + name;
                string dst_path = dirs[1] + name;
                if (!File.Exists(src_path))
                {
                    Console.WriteLine("missing: " + name);
                    missing.Add(name);
                    continue;
                }

                //Console.WriteLine("ok: " + src_path + " => " + dst_path);
                File.Copy(src_path, dst_path);
                ((BackgroundWorker)sender).ReportProgress(i);
            }

            Console.WriteLine("missing games: " + missing.Count);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}
