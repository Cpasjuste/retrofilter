using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

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

    [XmlIgnore] public Type InputType = Type.MameGame;

    public void Save(string path)
    {
        var root = InputType == Type.EmulationStation ? "gameList" : "datafile";
        if (InputType == Type.Mame2003) root = "mame";
        var xmlRoot = new XmlRootAttribute(root);
        var serializer = new XmlSerializer(typeof(DataFile), xmlRoot);
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        serializer.Serialize(writer, this);
    }

    public void Append(string path)
    {
        var db = Load(path);
        if (db == null) return;

        foreach (var game in Games)
        {
            var name = InputType == Type.EmulationStation
                ? Path.GetFileNameWithoutExtension(game.Path)
                : game.Name;
            var newGame = db.Games.FirstOrDefault(g => g.Name != null && g.Name.Equals(name));
            if (newGame == null) continue; // game not found in newly loaded db
            // parse each game properties to look for a new value
            foreach (var prop in newGame.GetType().GetProperties())
            {
                var newPropValue = prop.GetValue(newGame);
                var curPropValue = prop.GetValue(game);
                if (newPropValue != null && curPropValue == null) prop.SetValue(game, newPropValue);
            }
        }

        FilteredGames = new ObservableCollection<Game>(Games);
    }

    public static DataFile? Load(string path)
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
            return null;
        }

        if (dataFile.Games is not { Count: > 0 })
        {
            Console.WriteLine("Could not parse gamelist: no games found");
            return null;
        }

        // is datafile a mame "game" or "machine" nodes
        if (dataFile.Games[0] is Machine)
        {
            dataFile.InputType = Type.MameMachine;
        }

        // parse embedded catver, convert it to dictionary for faster search
        var cat = Utility.ReadAsset("catver_0.261.ini");
        var lines = cat.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var dic = lines.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
        foreach (var game in dataFile.Games)
        {
            var name = string.IsNullOrEmpty(game.Name) ? game.NameEs : game.Name;
            if (!dic.TryGetValue(name!, out var genre) && game.IsClone == "yes")
                dic.TryGetValue(game.CloneOf!, out genre);
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
            serializer.UnknownElement += (sender, args) =>
                Console.WriteLine("XmlSerializer: UnknownElement => {0}", args.Element.Name);
            serializer.UnknownAttribute += (sender, args) =>
                Console.WriteLine("XmlSerializer: UnknownAttribute => {0}", args.Attr.Name);
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