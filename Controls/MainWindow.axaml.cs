using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using RetroFilter.Sources;

namespace RetroFilter.Controls;

public partial class MainWindow : Window
{
    public DataFile Database { get; private set; } = new();
    public const DataFile.Type OutputType = DataFile.Type.EmulationStation;
    private CatVer? _catVer;

    public MainWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();

        GameGrid.AutoGeneratingColumn += OnAutoGeneratingColumn;
        GameGrid.LoadingRow += (sender, e) =>
        {
            _ = sender;
            if (e.Row.DataContext is not Game game) return;
            e.Row.Foreground = game.Missing == "yes" ? Brushes.Orange : Brushes.WhiteSmoke;
        };

        if (!Design.IsDesignMode) Task.Run(Load);
    }

    private async void Load()
    {
        // load cat/ver
        _catVer = new CatVer();

        // extract fbneo db if needed
        if (!File.Exists("fbneo.dat")) Utility.UnzipFromAsset("fbneo.zip", "fbneo.dat");

        // load fbneo db
        Database = DataFile.Load("fbneo.dat", _catVer);

        // refresh games grid
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            GameGrid.ItemsSource = Database.FilteredGames;
            UpdateHeader();
        });
    }

    private async void OnChooseDirectory(object? sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;
        var paths = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open a rom folder",
            AllowMultiple = false
        });

        if (paths.Count < 1) return;
        var path = paths[0].TryGetLocalPath();
        if (path == null || !Directory.Exists(path))
        {
            await ShowMessageBox("Oops", "could not open rom folder (" + path + ")");
            return;
        }

        // load gamelist.xml if exists
        if (File.Exists(path + "/gamelist.xml"))
        {
            // ask for props to override
            await LoadGameListDialog.ShowAsync(true);

            // parse props to override
            List<string> props = new();
            foreach (var item in LoadGameListDialogListBox.Items)
            {
                if (item is not CheckBox cb) continue;
                if (cb.IsChecked != null && cb.IsChecked.Value && cb.Content is string str)
                    props.Add(str);
            }

            Database.Append(path + "/gamelist.xml", _catVer!, props.ToArray());
        }

        // load roms from selected directory
        foreach (var file in Directory.GetFiles(path))
        {
            var romPath = Path.GetFileName(file);
            var game = Database.Games.FirstOrDefault(g => g.Path == romPath);
            if (game != null) game.Missing = "no";
        }

        GameGrid.ItemsSource = Database.FilteredGames;
        ButtonSaveDataFile.IsEnabled = true;
        UpdateHeader();
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs? e)
    {
        if (e == null) return;

        var headerName = e.Column.Header.ToString();
        if (string.IsNullOrEmpty(headerName)
            || headerName == "MameName"
            || headerName == "Description"
            || headerName == "Manufacturer"
            || headerName == "Year"
            || headerName == "VideoElement")
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
            //case "NameEs": e.Column.Header = "Name"; break;
            case "Desc" or "Description":
                e.Column.Width = new DataGridLength(300);
                break;
        }

        e.Column.IsReadOnly = false;
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

    private async void OnDataGridHeaderTextChanged(object? sender, TextChangedEventArgs e)
    {
        _ = sender;

        // get column header/field
        if (e.Source is not TextBox textBox) return;
        var header = Utility.FindParent<DataGridColumnHeader>(textBox);
        var field = (string)header?.Content!;

        // set ui waiting state
        GameGrid.Opacity = 0.5f;
        textBox.IsEnabled = false;
        await Task.Delay(1);

        // get filtering text
        var text = textBox.Text?.ToLower();

        // clear filtered game list
        Database.FilteredGames?.Clear();

        if (string.IsNullOrEmpty(text))
        {
            // very slow but prevent DataGrid sorting to be cleared
            foreach (var game in Database.Games) Database.FilteredGames?.Add(game);
            textBox.IsEnabled = true;
            textBox.IsVisible = false;
        }
        else
        {
            // add games to filtered list
            foreach (var game in Database.Games)
            {
                if (game.GetType().GetProperty(field) is not { } prop) continue;
                if (prop.GetValue(game) is string value && value.ToLower().Contains(text))
                {
                    Database.FilteredGames?.Add(game);
                }
            }

            textBox.IsEnabled = true;
            textBox.Focus();
        }

        GameGrid.Opacity = 1f;

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
        var missing = Database.FilteredGames?.Count(g => g.Missing == "yes");
        HeaderText.Text = Database.Header.Name + " | " + Database.Header.Description
                          + " (" + Database.FilteredGames?.Count + " games, missing: " + missing + ")";
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