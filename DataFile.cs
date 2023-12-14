using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RetroFilter;

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

    private ObservableCollection<Game> _games = new();

    [XmlElement("game", typeof(Game))]
    [XmlElement("machine", typeof(Machine))]
    public ObservableCollection<Game> Games
    {
        get => _games;
        set => SetField(ref _games, value);
    }

    [XmlIgnore]
    public Type DataType = Type.MameGame;

    public List<string> GetFilteredColumns()
    {
        var filters = new List<string>();

        // hide non handled items
        filters.AddRange(new List<string>
        {
            "IsBiosString", "IsDeviceString", "Driver", "BiosSets",
            "Roms", "DeviceRefs", "Samples", "Disks", "Chips", "Video",
            "Sound", "Input", "DipSwitches"
        });

        if (DataType != Type.EmulationStation)
        {
            filters.AddRange(new List<string>
            {
                "NameEs", "Desc", "Source", "Image", "Thumbnail", "ReleaseDate",
                "Path", "Developer", "Region", "RomType", "Id",
                "Publisher", "Rating", "Players", "Hash"
            });
        }

        if (DataType == Type.MameGame)
        {
            filters.AddRange(new List<string>
                { "SourceFile", "IsDevice", "Runnable" });
        }
        else if (DataType == Type.Mame2003)
        {
            filters.AddRange(new List<string>
                { "SourceFile" });
        }
        else if (DataType == Type.EmulationStation)
        {
            filters.AddRange(new List<string>
            {
                "Name", "SourceFile", "IsDevice", "Runnable",
                "IsBios", "Year", "Description", "Manufacturer",
                "RomOf", "CloneOf", "IsRom", "IsClone"
            });
        }

        return filters;
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
                dataFile.DataType = Type.Mame2003;
            }
        }

        if (dataFile == null)
        {
            // gamelist.xml
            Console.WriteLine("trying gamelist.xml format...");
            if ((dataFile = Load(path, "gameList")) != null)
            {
                dataFile.DataType = Type.EmulationStation;
            }
        }

        if (dataFile == null)
        {
            Console.WriteLine("Could not parse file: " + path);
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
            dataFile.DataType = Type.MameMachine;
        }

        // parse embedded catver, convert it to dictionary for faster search
        var cat = Utility.ReadAsset("catver_0.261.ini");
        var lines = cat.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var dic = lines.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
        foreach (var game in dataFile.Games)
        {
            var name = string.IsNullOrEmpty(game.Name) ? game.NameEs : game.Name;
            if (!dic.TryGetValue(name, out var genre) && game.IsClone) dic.TryGetValue(game.CloneOf, out genre);
            if (!string.IsNullOrEmpty(genre)) game.Genre = genre;
        }

        Console.WriteLine("database loaded: type is " + dataFile.DataType + ", games: " + dataFile.Games.Count);
        return dataFile;
    }

    /*
    public static bool Save(DataFile dataFile, ItemCollection collection, string path)
    {
        try
        {
            string root = dataFile.type == Type.EmulationStation ? "gameList" : "datafile";
            if (dataFile.type == Type.Mame2003)
            {
                root = "mame";
            }

            XmlRootAttribute xmlRoot = new XmlRootAttribute(root);
            XmlSerializer serializer = new XmlSerializer(typeof(DataFile), xmlRoot);
            DataFile df = new DataFile();
            df.Header = dataFile.type == Type.EmulationStation ? null : dataFile.Header;
            // only save visible games
            df.Games = new List<Game>(collection.OfType<Game>());
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, df);
            writer.Close();
        }
        catch
        {
            Console.WriteLine("Could not parse gamelist: " + path);
            return false;
        }

        return true;
    }
    */

    private static DataFile? Load(string path, string root)
    {
        DataFile? dataFile = null;

        try
        {
            var serializer = new XmlSerializer(typeof(DataFile), new XmlRootAttribute { ElementName = root });
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