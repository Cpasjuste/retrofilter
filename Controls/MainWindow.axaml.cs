using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
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
        _ = sender;
        _ = e;
        foreach (Game game in GamesGrid.ItemsSource) game.Locked = true;
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private void btnUnlockAll_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        foreach (Game game in GamesGrid.ItemsSource) game.Locked = false;
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private async void btnLoadDat_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;

        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Database File",
            AllowMultiple = false
        });

        if (files.Count < 1) return;

        var path = files[0].TryGetLocalPath();
        if (path == null)
        {
            await ShowMessageBox("Oops", "Could not open database from " + files[0].Path);
            return;
        }

        var db = DataFile.Load(path);
        if (db == null)
        {
            await ShowMessageBox("Oops", "Could not load database from " + files[0].Path);
            return;
        }

        DataFile = db;
        GamesGrid.ItemsSource = DataFile.Games;
        HeaderText.Text = DataFile.Header.Name + " | " + DataFile.Header.Description
                          + " (" + DataFile.Games.Count + " games)";
    }

    private void btnSaveDat_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        throw new NotImplementedException();
    }

    private void btnProcessFolder_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        throw new NotImplementedException();
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

    private Task<object> ShowMessageBox(string title, string content, bool indeterminate = false)
    {
        MainTaskDialog.Header = title;
        MainTaskDialog.Content = Environment.NewLine + content;
        MainTaskDialog.ShowProgressBar = indeterminate;
        MainTaskDialog.SetProgressBarState(0, TaskDialogProgressState.Indeterminate);
        foreach (var btn in MainTaskDialog.Buttons)
        {
            btn.IsEnabled = !indeterminate;
        }

        return MainTaskDialog.ShowAsync(true);
    }
}