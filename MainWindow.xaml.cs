using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace RetroFilter
{
    public partial class MainWindow : MetroWindow
    {
        GameList gameList;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            loadDat.Visibility = Visibility.Visible;
            headerPanel.Visibility = Visibility.Hidden;
            gamesGrid.Visibility = Visibility.Hidden;
            gamesGrid.AutoGeneratingColumn += mameGrid_AutoGeneratingColumn;
            gamesGrid.UnloadingRow += mameGrid_UnLoadingRow;
        }

        void mameGrid_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            headerText.Content = gameList.dataFile.Header.Name + ": "
                        + gameList.dataFile.Header.Description + " ("
                        + gameList.dataFile.gamesCollection.Count + " games)";
        }

        void mameGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string name = e.Column.Header.ToString();

            // hide not handled columns (DataFile child classes)
            if (name == "Driver" || name == "BiosSets" || name == "Roms" || name == "DeviceRefs" || name == "Samples")
            {
                e.Column.Visibility = Visibility.Hidden;
            }

            // hide non used columns
            if (gameList.dataFile.Games[0].GetType().Name != "MameGame")
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
                gameList = new GameList();
                if (gameList.Load(openFileDialog.FileName))
                {
                    headerText.Content = gameList.dataFile.Header.Name + ": "
                        + gameList.dataFile.Header.Description + " (" + gameList.dataFile.Games.Count + " games)";
                    gamesGrid.ItemsSource = gameList.dataFile.gamesCollection;
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
            if (gameList != null && gameList.dataFile != null && gameList.dataFile.Games.Count > 0)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Mame DAT (*.dat)|*.dat|All files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (!gameList.Save(dialog.FileName))
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
    }
}
