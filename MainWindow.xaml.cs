using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace RetroFilter
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            mameGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnLoadDat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mame DAT (*.dat)|*.dat|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                GameList gameList = new GameList();
                if (gameList.Load(openFileDialog.FileName))
                {
                    mameGrid.ItemsSource = gameList.games;
                    LoadDat.Visibility = System.Windows.Visibility.Hidden;
                    mameGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    LoadDat.Visibility = System.Windows.Visibility.Visible;
                    mameGrid.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
    }
}
