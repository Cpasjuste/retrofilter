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
        if (string.IsNullOrEmpty(headerName))
        {
            e.Column.IsVisible = false;
            e.Cancel = true;
            return;
        }

        // do not show columns with no content or not a string prop
        var hasData = Database.Games.Any(game => game.GetType().GetProperty(headerName)?.GetValue(game) is string);
        if (!hasData)
        {
            e.Column.IsVisible = false;
            e.Cancel = true;
            return;
        }

        switch (headerName)
        {
            case "NameEs":
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
            FileTypeFilter = new[] { Utility.DatabaseFilter, FilePickerFileTypes.All }
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
        ButtonSaveDataFile.IsEnabled = true;
        UpdateHeader();
    }

    private void btnProcessFolder_Click(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        throw new NotImplementedException();
    }

    private void OnSaveDataFile(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        // TODO: file dialog
        Database.Save("test.dat");
    }

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

        UpdateHeader();
    }

    private void OnDataGridHeaderTextChanged(object? sender, TextChangedEventArgs e)
    {
        _ = sender;

        // get column header/field
        if (e.Source is not TextBox textBox) return;
        var header = Utility.FindParent<DataGridColumnHeader>(textBox);
        var field = (string)header?.Content!;

        // get filtering text
        var text = textBox.Text?.ToLower();

        // clear filtering if search text is null or empty
        if (string.IsNullOrEmpty(text))
        {
            Database.FilteredGames = new ObservableCollection<Game>(Database.Games);
            GameGrid.ItemsSource = Database.FilteredGames;
            textBox.IsVisible = false;
            UpdateHeader();
            return;
        }

        // clear filtered game list
        Database.FilteredGames?.Clear();

        // add games to filtered list
        foreach (var game in Database.Games)
        {
            if (game.GetType().GetProperty(field) is not { } prop) continue;
            if (prop.GetValue(game) is string value && value.ToLower().Contains(text))
            {
                Database.FilteredGames?.Add(game);
            }
        }

        // update header text
        UpdateHeader();
    }

    private void OnGameGridHeaderTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        _ = e;
        if (sender is not TextBox tb) return;
        if (string.IsNullOrEmpty(tb.Text)) tb.IsVisible = false;
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

    private void UpdateHeader()
    {
        // update header
        HeaderText.Text = Database.Header.Name + " | " + Database.Header.Description
                          + " (" + Database.FilteredGames?.Count + " games)";
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