using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using DataGridExtensions;
using System;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;

namespace RetroFilter
{
    public partial class MainWindow : MetroWindow
    {
        public DataFile dataFile;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            loadDat.Visibility = Visibility.Visible;
            headerPanel.Visibility = Visibility.Hidden;
            gamesGrid.Visibility = Visibility.Hidden;
            gamesGrid.AutoGeneratingColumn += OnAutoGeneratingColumn;
            gamesGrid.LoadingRow += OnLoadingRow;
            gamesGrid.UnloadingRow += OnUnloadingRow;
        }

        private void gamesGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key && grid.SelectedItems.Count > 0)
            {
                List<Game> games = new List<Game>();
                foreach (Game game in grid.SelectedItems)
                {
                    if (!game.Locked)
                    {
                        games.Add(game);
                    }
                }
                foreach (Game game in games)
                {
                    dataFile.gamesCollection.Remove(game);
                }
            }
            // update header
            headerText.Content = dataFile.Header.Name + ": "
                       + dataFile.Header.Description + " ("
                       + gamesGrid.Items.Count + " games)";
            e.Handled = true;
        }

        void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            headerText.Content = dataFile.Header.Name + ": "
                        + dataFile.Header.Description + " ("
                        + gamesGrid.Items.Count + " games)";
        }

        void OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            headerText.Content = dataFile.Header.Name + ": "
                        + dataFile.Header.Description + " ("
                        + gamesGrid.Items.Count + " games)";
        }

        void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string name = e.Column.Header.ToString();
            // hide unhandled columns (DataFile child classes)
            if (name == "IsBiosInternal" || name == "Driver" || name == "BiosSets"
                || name == "Roms" || name == "DeviceRefs" || name == "Samples" || name == "IsDeleteEnabled")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
            // hide non used columns
            if (dataFile.Games[0].GetType().Name != "MameGame")
            {
                if (name == "SourceFile" || name == "IsDevice" || name == "Runnable")
                {
                    e.Column.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnLoadDat_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mame DAT (*.dat)|*.dat|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (LoadDatafile(openFileDialog.FileName))
                {
                    gamesGrid.ItemsSource = dataFile.gamesCollection;
                    loadDat.Visibility = Visibility.Hidden;
                    headerPanel.Visibility = Visibility.Visible;
                    gamesGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    this.ShowMessageAsync("Oups", "Something went wrong with this file...");
                    loadDat.Visibility = Visibility.Visible;
                    headerPanel.Visibility = Visibility.Hidden;
                    gamesGrid.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnSaveDat_Click(object sender, RoutedEventArgs e)
        {
            if (dataFile != null && dataFile.Games.Count > 0)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Mame DAT (*.dat)|*.dat|All files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (!SaveDatafile(dialog.FileName))
                    {
                        this.ShowMessageAsync("Oups", "Something went wrong with this file...");
                    }
                }
            }
        }

        private void btnSupport_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/cpasjuste");
        }

        public bool LoadDatafile(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                StreamReader reader = new StreamReader(path);
                dataFile = (DataFile)serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }

            if (dataFile.Games.Count <= 0)
            {
                Console.WriteLine("Could not parse gamelist: no games found");
                return false;
            }

            // parse embedded catver, convert it to dictionnary for faster search
            string catver = GetEmbeddedResource("catver_0.214.ini");
            string[] catverLines = catver.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> catverDic = catverLines.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
            foreach (Game game in dataFile.Games)
            {
                if (!catverDic.TryGetValue(game.Name, out string genre) && game.IsClone)
                {
                    catverDic.TryGetValue(game.CloneOf, out genre);
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    game.Genre = genre;
                }
            }

            // finally, create ObservableCollection for datagrid
            dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Games);

            return true;
        }

        public bool SaveDatafile(string path)
        {
            try
            {
                TextWriter writer = new StreamWriter(path);
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                dataFile.Games = new List<Game>(gamesGrid.Items.OfType<Game>());
                serializer.Serialize(writer, dataFile);
                writer.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }
            return true;
        }

        private string GetEmbeddedResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            resourceName = assembly.GetName().Name + "." +
                resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
