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
            mameGrid.Visibility = Visibility.Hidden;
            mameGrid.AutoGeneratingColumn += mameGrid_AutoGeneratingColumn;
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
                    mameGrid.ItemsSource = gameList.dataFile.gamesCollection;
                    LoadDat.Visibility = Visibility.Hidden;
                    mameGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    this.ShowMessageAsync("Oups", "Something went wrong with this file...");
                    LoadDat.Visibility = Visibility.Visible;
                    mameGrid.Visibility = Visibility.Hidden;
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
