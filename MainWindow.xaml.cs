using System.Windows;
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
            InitializeComponent();
            mameGrid.Visibility = Visibility.Hidden;
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
