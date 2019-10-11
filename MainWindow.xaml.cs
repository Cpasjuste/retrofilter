﻿using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;

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

        private void gamesGrid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (e.Command == DataGrid.DeleteCommand)
            {
                int index = gamesGrid.SelectedIndex;
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

                // update header
                headerName.Text = dataFile.Header.Name;
                headerDesc.Text = dataFile.Header.Description;
                headerGameCount.Content = "(" + gamesGrid.Items.Count + " games)";

                // select next row
                if (index >= gamesGrid.Items.Count)
                {
                    index = gamesGrid.Items.Count - 1;
                }
                if (index >= 0)
                {
                    object item = gamesGrid.Items[index];
                    if (item != null)
                    {
                        gamesGrid.SelectedItem = item;
                        gamesGrid.ScrollIntoView(item);
                        DataGridRow row = (DataGridRow)gamesGrid.ItemContainerGenerator.ContainerFromIndex(index);
                        if (row != null)
                        {
                            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        }
                    }
                }

                // ok
                e.Handled = true;
            }
        }

        void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            headerName.Text = dataFile.Header.Name;
            headerDesc.Text = dataFile.Header.Description;
            headerGameCount.Content = "(" + gamesGrid.Items.Count + " games)";
        }

        void OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            headerName.Text = dataFile.Header.Name;
            headerDesc.Text = dataFile.Header.Description;
            headerGameCount.Content = "(" + gamesGrid.Items.Count + " games)";
        }

        void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string name = e.Column.Header.ToString();

            if (dataFile.GetFilteredColumns().Contains(name))
            {
                e.Column.Visibility = Visibility.Hidden;
            }

            if (name == "NameES")
            {
                e.Column.Header = "Name";
            }
            else if (name == "Desc")
            {
                e.Column.Width = 400;
            }
        }

        private void btnLockAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Game game in gamesGrid.Items)
            {
                game.Locked = true;
            }

            CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
        }

        private void btnUnlockAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Game game in gamesGrid.Items)
            {
                game.Locked = false;
            }

            CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
        }

        private void btnLoadDat_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mame / ES (*.dat/*.xml)|*.dat;*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                dataFile = DataFile.Load(openFileDialog.FileName);
                if (dataFile != null)
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
                dialog.Filter = "Mame / ES (*.dat/*.xml)|*.dat;*.xml|All files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (!DataFile.Save(dataFile, gamesGrid.Items, dialog.FileName))
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
