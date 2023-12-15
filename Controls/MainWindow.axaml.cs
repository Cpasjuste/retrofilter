using System;
using System.Collections.ObjectModel;
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
    public DataFile Database { get; private set; } = new();

    public MainWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();

        GameGrid.AutoGeneratingColumn += OnAutoGeneratingColumn;

        if (Design.IsDesignMode)
        {
            Database.FilteredGames = new ObservableCollection<Game>
            {
                new() { Name = "Super Game" },
                new() { Name = "Super Game 2" }
            };
            GameGrid.ItemsSource = Database.FilteredGames;
        }
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs? e)
    {
        if (e == null) return;

        var headerName = e.Column.Header.ToString();
        if (string.IsNullOrEmpty(headerName) || Database.GetFilteredColumns().Contains(headerName))
        {
            e.Column.IsVisible = false;
            e.Cancel = true;
            return;
        }

        switch (headerName)
        {
            case "NameES":
                e.Column.Header = "Name";
                break;
            case "Desc" or "Description":
                e.Column.Width = new DataGridLength(300);
                break;
        }

        e.Column.IsReadOnly = false;
    }

    private void btnLockAll_Click(object? sender, RoutedEventArgs? e)
    {
        _ = sender;
        _ = e;
        //foreach (Game game in GameGrid.ItemsSource) game.Locked = true;
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private void btnUnlockAll_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        //foreach (Game game in GameGrid.ItemsSource) game.Locked = false;
        //CollectionViewSource.GetDefaultView(gamesGrid.ItemsSource).Refresh();
    }

    private static FilePickerFileType GameDatabase { get; } = new("Mame / EmulationStation (*.dat/*.xml)")
    {
        Patterns = new[] { "*.dat", "*.xml" },
        MimeTypes = new[] { "dat/*" }
    };

    private async void OnLoadDatFile(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;

        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Database File",
            AllowMultiple = false,
            FileTypeFilter = new[] { GameDatabase, FilePickerFileTypes.All }
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

        Database = db;
        GameGrid.ItemsSource = Database.FilteredGames;
        HeaderText.Text = Database.Header.Name + " | " + Database.Header.Description
                          + " (" + Database.Games.Count + " games)";
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

    public async void SortDataGrid(object? headerName)
    {
        //Console.WriteLine("SortDataGrid: " + GameGrid.Columns[0].Header);
        var col = GameGrid.Columns.FirstOrDefault(c => c.Header == headerName);
        if (col is not DataGridTextColumn tc) return;
        Console.WriteLine("SortDataGrid: column == " + tc.Header);

        //tc.SortMemberPath = col.Header.ToString();
        //tc.Sort();

        // crappy working, Sort doesn't work when CanUserSortColumns is false...
        GameGrid.CanUserSortColumns = true;
        await Task.Delay(1);
        GameGrid.CanUserSortColumns = false;
    }

    // TODO: async
    private void OnDataGridKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete) return;
        if (sender is not DataGrid grid) return;
        if (grid.SelectedItems == null) return;

        var games = grid.SelectedItems.Cast<Game>().ToList();
        foreach (var game in games)
        {
            Database.Games.Remove(game);
            Database.FilteredGames?.Remove(game);
        }

        HeaderText.Text = Database.Header.Name + " | " + Database.Header.Description
                          + " (" + Database.FilteredGames?.Count + " games)";
    }

    private void OnDataGridHeaderTextChanged(object? sender, TextChangedEventArgs e)
    {
        _ = sender;
        if (e.Source == null) return;

        // get column header name
        var textBox = (TextBox)e.Source;
        var header = Utility.FindParent<DataGridColumnHeader>(textBox);
        var field = (string)header?.Content!;

        // get filtering text
        var text = ((TextBox)e.Source).Text;

        // filter games
        var games = string.IsNullOrEmpty(text)
            ? Database.Games
            : Database.Games.Where(game =>
            {
                var prop = game.GetType().GetProperty(field);
                if (prop == null) return false;
                var value = (string)prop.GetValue(game)!;
                return value.ToLower().Contains(text.ToLower());
            });

        // update games
        Database.FilteredGames?.Clear();
        foreach (var game in games) Database.FilteredGames?.Add(game);

        if (string.IsNullOrEmpty(text)) textBox.IsVisible = false;

        // update header
        HeaderText.Text = Database.Header.Name + " | " + Database.Header.Description
                          + " (" + Database.FilteredGames?.Count + " games)";

        Console.WriteLine("OnDataGridHeaderTextChanged: text = {0}, games: {1}", text, Database.FilteredGames?.Count);
    }

    private void OnGameGridHeaderTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox tb) return;
        if (string.IsNullOrEmpty(tb.Text)) tb.IsVisible = false;
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