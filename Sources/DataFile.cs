using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using RetroFilter.Controls;

namespace RetroFilter.Sources;

[Serializable]
[XmlRoot("datafile", Namespace = "", IsNullable = false)]
public class DataFile : INotifyPropertyChanged
{
    public enum Type
    {
        MameGame,
        MameMachine,
        Mame2003,
        EmulationStation
    }

    [XmlIgnore]
    public Type InputType = Type.MameGame;

    private Header _header = new();

    [XmlElement(ElementName = "header")]
    public Header Header
    {
        get => _header;
        set => SetField(ref _header, value);
    }

    [XmlElement("game", typeof(Game))]
    [XmlElement("machine", typeof(Machine))]
    public ObservableCollection<Game> Games { get; set; } = new();

    private ObservableCollection<Game>? _filteredGames;

    [XmlIgnore]
    public ObservableCollection<Game>? FilteredGames
    {
        get => _filteredGames;
        set => SetField(ref _filteredGames, value);
    }

    public void Save(string path)
    {
        var root = MainWindow.OutputType == Type.EmulationStation ? "gameList" : "datafile";
        if (MainWindow.OutputType == Type.Mame2003) root = "mame";
        var xmlRoot = new XmlRootAttribute(root);
        var serializer = new XmlSerializer(typeof(DataFile), xmlRoot);
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        serializer.Serialize(writer, this);
    }

    public void Append(string path, CatVer catVer, string[] propsToOverride)
    {
        var db = Load(path, catVer);
        foreach (var game in db.Games)
        {
            var curGame = Games.FirstOrDefault(g => g.Path == game.Path);
            if (curGame == null) continue;

            // parse each game properties to look for a new value
            foreach (var prop in game.GetType().GetProperties())
            {
                var newPropValue = prop.GetValue(game);
                if (newPropValue == null) continue;
                var curPropValue = prop.GetValue(curGame);
                if (curPropValue == null
                    || prop.Name == nameof(Game.VideoElement)
                    || propsToOverride.Contains(prop.Name))
                    prop.SetValue(curGame, newPropValue);
            }
        }

        FilteredGames = new ObservableCollection<Game>(Games);
    }

    public static DataFile Load(string path, CatVer catVer)
    {
        // mame.dat
        var dataFile = Load(path, "datafile");
        if (dataFile == null)
        {
            // mame2003-plus.xml
            Console.WriteLine("trying mame2003 format...");
            if ((dataFile = Load(path, "mame")) != null)
            {
                dataFile.InputType = Type.Mame2003;
            }
        }

        if (dataFile == null)
        {
            // gamelist.xml
            Console.WriteLine("trying gamelist.xml format...");
            if ((dataFile = Load(path, "gameList")) != null)
            {
                dataFile.InputType = Type.EmulationStation;
            }
        }

        if (dataFile == null)
        {
            Console.WriteLine("Could not parse database file from " + path);
            return new DataFile();
        }

        if (dataFile.Games is not { Count: > 0 })
        {
            Console.WriteLine("Could not parse gamelist: no games found");
            return new DataFile();
        }

        // is datafile a mame "game" or "machine" nodes
        if (dataFile.Games[0] is Machine)
        {
            dataFile.InputType = Type.MameMachine;
        }

        // map cat/ver
        foreach (var game in dataFile.Games)
        {
            var name = game.MameName ?? Path.GetFileNameWithoutExtension(game.Path);
            if (!catVer.Mapping.TryGetValue(name!, out var genre) && game.IsClone == "yes")
                catVer.Mapping.TryGetValue(game.CloneOf!, out genre);
            if (!string.IsNullOrEmpty(genre)) game.Genre = genre;
        }

        // filtered game list
        dataFile.FilteredGames = new ObservableCollection<Game>(dataFile.Games);

        Console.WriteLine("database loaded: type is " + dataFile.InputType + ", games: " + dataFile.Games.Count);
        return dataFile;
    }

    private static DataFile? Load(string path, string root)
    {
        DataFile? dataFile = null;

        try
        {
            var serializer = new XmlSerializer(typeof(DataFile), new XmlRootAttribute { ElementName = root });
            // TODO: fix a few elements/attributes reported as "unknown" while they seems fine in game class...
            /*
            serializer.UnknownElement += (sender, args) =>
                Console.WriteLine("XmlSerializer: UnknownElement => {0} (line {1})",
                    args.Element.Name, args.LineNumber);
            serializer.UnknownAttribute += (sender, args) =>
                Console.WriteLine("XmlSerializer: UnknownAttribute => {0} (line {1})",
                    args.Attr.Name, args.LineNumber);
            */
            var reader = new StreamReader(path);
            dataFile = (DataFile)serializer.Deserialize(reader)!;
            reader.Close();
        }
        catch
        {
            Console.WriteLine("Could not parse file: " + path);
        }

        return dataFile;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}