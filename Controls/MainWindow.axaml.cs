using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RetroFilter.Sources;

namespace RetroFilter.Controls;

public partial class MainWindow : Window
{
    public DataFile DataFile { get; private set; } = new();

    public MainWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();

        GamesGrid.AutoGeneratingColumn += OnAutoGeneratingColumn;
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs? e)
    {
        if (e == null) return;

        var headerName = e.Column.Header.ToString();
        if (headerName == null) return;
        if (DataFile.GetFilteredColumns().Contains(e.Column.Header.ToString()!))
        {
            //Console.WriteLine("OnAutoGeneratingColumn: disabling column: " + e.Column.Header);
            e.Column.IsVisible = false;
            e.Cancel = true;
            return;
        }

        if (headerName == "nameES")
        {
            e.Column.Header = "Name";
        }
        else if (headerName is "Desc" or "Description")
        {
            e.Column.Width = new DataGridLength(300);
        }

        e.Column.IsReadOnly = false;
    }

    private void btnLockAll_Click(object? sender, RoutedEventArgs? e)
    {
        foreach (Game game in GamesGrid.ItemsSource)
        {
            game.Locked = true;
        }
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private void btnUnlockAll_Click(object? sender, RoutedEventArgs e)
    {
        foreach (Game game in GamesGrid.ItemsSource)
        {
            game.Locked = false;
        }
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private async void btnLoadDat_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Database File",
            AllowMultiple = false
        });

        if (files.Count < 1) return;

        Console.WriteLine("btnLoadDat_Click: " + files[0].TryGetLocalPath());

        DataFile = DataFile.Load(files[0].TryGetLocalPath()!) ?? new DataFile();
        if (DataFile != null)
        {
            GamesGrid.ItemsSource = DataFile.Games;
            HeaderText.Text = DataFile.Header.Name + " | " + DataFile.Header.Description
                              + " (" + DataFile.Games.Count + " games)";
            //MainGrid.IsVisible = true;
        }
        else
        {
            //this.ShowMessageAsync("Oups", "Something went wrong with this file...");
            //loadDat.Visibility = Visibility.Visible;
            //headerPanel.Visibility = Visibility.Hidden;
            //gamesGrid.Visibility = Visibility.Hidden;
        }

        /*
        // Open reading stream from the first file.
        await using var stream = await files[0].OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        // Reads all the content of file as a text.
        var fileContent = await streamReader.ReadToEndAsync();
        */

        /*
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Mame / ES (*.dat/*.xml)|*.dat;*.xml|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            dataFile = DataFile.Load(openFileDialog.FileName);
            if (dataFile != null)
            {
                gamesGrid.ItemsSource = dataFile.Games;
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
        */
    }

    private void btnSaveDat_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void btnProcessFolder_Click(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    // TODO: async
    private void GamesGrid_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete) return;
        if (sender is not DataGrid grid) return;
        if (grid.SelectedItems == null) return;

        var games = grid.SelectedItems.Cast<Game>().ToList();
        foreach (var game in games)
        {
            DataFile.Games.Remove(game);
        }
    }
}