using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using RetroFilter.Controls;

namespace RetroFilter.Sources;

// xml root: datafile, xml elements names: "machine" (Mame)
[Serializable]
public class Machine : Game
{
}

// xml root: datafile, xml elements names: "game" (Mame/Es)
[Serializable]
public class Game
{
    private static bool IsEs => MainWindow.OutputType == DataFile.Type.EmulationStation;

    //[XmlIgnore]
    //public bool Locked { get; set; }

    [XmlIgnore] public string? Missing { get; set; } = "yes";

    // ES
    [XmlAttribute("id")] public string? Id { get; set; }
    public bool ShouldSerializeId() => Id != null && IsEs;

    [XmlAttribute("source")] public string? Source { get; set; }
    public bool ShouldSerializeSource() => Source != null && IsEs;

    private string? _path;

    // ES
    [XmlElement("path")]
    public string? Path
    {
        get => _path ?? MameName + ".zip";
        set => _path = value;
    }

    public bool ShouldSerializePath() => Path != null && IsEs;

    private string? _name;

    // ES
    [XmlElement("name")]
    public string? Name
    {
        get => _name ?? Description;
        set => _name = value;
    }

    public bool ShouldSerializeName() => Name != null && IsEs;

    [XmlAttribute("name")] public string? MameName { get; set; }
    public bool ShouldSerializeMameName() => MameName != null && !IsEs;

    // ES
    private string? _desc;

    [XmlElement("desc")]
    public string? Desc
    {
        get => _desc ?? Description;
        set => _desc = value;
    }

    public bool ShouldSerializeDesc() => Desc != null && IsEs;

    [XmlElement("description")] public string? Description { get; set; }
    public bool ShouldSerializeDescription() => Description != null && !IsEs;

    private string? _releaseDate;

    // ES
    [XmlElement("releasedate")]
    public string? ReleaseDate
    {
        get => Utility.IsDigits(Year) ? Year + "0101T000000" : _releaseDate;
        set => _releaseDate = value;
    }

    public bool ShouldSerializeReleaseDate() => ReleaseDate != null && IsEs;

    [XmlElement("year")] public string? Year { get; set; }
    public bool ShouldSerializeYear() => Year != null && !IsEs;

    // ES
    private string? _developer;

    [XmlElement("developer")]
    public string? Developer
    {
        get => _developer ?? Manufacturer;
        set => _developer = value;
    }

    public bool ShouldSerializeDeveloper() => Developer != null && IsEs;

    private string? _publisher;

    [XmlElement("publisher")]
    public string? Publisher
    {
        get => _publisher ?? Manufacturer;
        set => _publisher = value;
    }

    public bool ShouldSerializePublisher() => Publisher != null && IsEs;

    [XmlElement("manufacturer")] public string? Manufacturer { get; set; }
    public bool ShouldSerializeManufacturer() => Manufacturer != null && !IsEs;

    [XmlElement("genre")] public string? Genre { get; set; }
    public bool ShouldSerializeGenre() => Genre != null && IsEs;

    // ES
    [XmlElement("genreid")] public string? GenreId { get; set; }
    public bool ShouldSerializeGenreId() => GenreId != null && IsEs;

    // ES
    [XmlElement("players")] public string? Players { get; set; }
    public bool ShouldSerializePlayers() => Players != null && IsEs;

    [XmlElement("hash")] public string? Hash { get; set; }
    public bool ShouldSerializeHash() => Hash != null && IsEs;

    [XmlElement("image")] public string? Image { get; set; }
    public bool ShouldSerializeImage() => Image != null && IsEs;

    [XmlElement("thumbnail")] public string? Thumbnail { get; set; }
    public bool ShouldSerializeThumbnail() => Thumbnail != null && IsEs;

    private XmlElement? _videoElement;

    // video element can either be a string (es video path) or video class (mame)
    [XmlAnyElement("video")]
    public XmlElement? VideoElement
    {
        get => _videoElement;
        set
        {
            _videoElement = value;
            if (_videoElement == null) return;
            if (_videoElement.HasAttributes) // this is a mame video element (video class)
            {
                MameVideo = new Video
                {
                    Screen = _videoElement.GetAttribute("screen"),
                    Type = _videoElement.GetAttribute("type"),
                    Orientation = _videoElement.GetAttribute("orientation"),
                    Width = _videoElement.GetAttribute("width"),
                    Height = _videoElement.GetAttribute("height"),
                    AspectX = _videoElement.GetAttribute("aspectx"),
                    AspectY = _videoElement.GetAttribute("aspecty"),
                    Refresh = _videoElement.GetAttribute("refresh")
                };
            }
            else // this is an es video path
            {
                Video = _videoElement.InnerText;
            }
        }
    }

    [XmlIgnore] public string? Video { get; set; }

    [XmlElement("comment")] public string? Comment { get; set; }
    public bool ShouldSerializeComment() => Comment != null && !IsEs;

    // custom props for DataGrid
    [XmlIgnore] public string? Resolution => MameVideo != null ? MameVideo?.Width + " x " + MameVideo?.Height : null;

    // custom props for DataGrid
    [XmlIgnore] public string? Orientation => MameVideo?.Orientation;

    [XmlAttribute("isbios")] public string? IsBios { get; set; }
    public bool ShouldSerializeIsBios() => IsBios != null && !IsEs;

    [XmlAttribute("isdevice")] private string? IsDevice { get; set; }
    public bool ShouldSerializeIsDevice() => IsDevice != null && !IsEs;

    [XmlAttribute("ismechanical")] private string? IsMechanical { get; set; }
    public bool ShouldSerializeIsMechanical() => IsMechanical != null && !IsEs;

    [XmlAttribute("runnable")] public string? Runnable { get; set; }
    public bool ShouldSerializeRunnable() => Runnable != null && !IsEs;

    // ES
    [XmlElement("rating")] public string? Rating { get; set; }
    public bool ShouldSerializeRating() => Rating != null && IsEs;

    [XmlElement("region")] public string? Region { get; set; }
    public bool ShouldSerializeRegion() => Region != null && IsEs;

    [XmlAttribute("romof")] public string? RomOf { get; set; }
    public bool ShouldSerializeRomOf() => RomOf != null && !IsEs;

    [XmlAttribute("cloneof")] public string? CloneOf { get; set; }
    public bool ShouldSerializeCloneOf() => CloneOf != null && !IsEs;

    // mame2003
    [XmlAttribute("sampleof")] public string? SampleOf { get; set; }
    public bool ShouldSerializeSampleOf() => SampleOf != null && !IsEs;

    [XmlIgnore] public string? IsClone => string.IsNullOrEmpty(CloneOf) ? null : "yes";

    [XmlAttribute("sourcefile")] public string? SourceFile { get; set; }
    public bool ShouldSerializeSourceFile() => SourceFile != null && !IsEs;

    // ES
    [XmlElement("romtype")] public string? RomType { get; set; }
    public bool ShouldSerializeRomType() => RomType != null && IsEs;

    [XmlElement("biosset")] public List<BiosSet>? BiosSets { get; set; }
    public bool ShouldSerializeBiosSets() => BiosSets != null && !IsEs;

    [XmlElement("rom")] public List<Rom>? Roms { get; set; }
    public bool ShouldSerializeRoms() => Roms != null && !IsEs;

    [XmlElement("softwarelist")] public List<SoftwareList>? SoftwareLists { get; set; }
    public bool ShouldSerializeSoftwareList() => SoftwareLists != null && !IsEs;

    [XmlElement("device_ref")] public List<DeviceRef>? DeviceRefs { get; set; }
    public bool ShouldSerializeDeviceRefs() => DeviceRefs != null && !IsEs;

    [XmlElement("sample")] public List<Sample>? Samples { get; set; }
    public bool ShouldSerializeSamples() => Samples != null && !IsEs;

    [XmlElement("driver")] public Driver? Driver { get; set; }
    public bool ShouldSerializeDriver() => Driver != null && !IsEs;

    [XmlIgnore] public string? Status => Driver?.Status;

    // mame2003
    [XmlElement("disk")] public List<Disk>? Disks { get; set; }
    public bool ShouldSerializeDisks() => Disks != null && !IsEs;

    [XmlElement("chip")] public List<Chip>? Chips { get; set; }
    public bool ShouldSerializeChips() => Chips != null && !IsEs;

    [XmlIgnore] public Video? MameVideo { get; set; }
    public bool ShouldSerializeMameVideo() => MameVideo != null && !IsEs;

    [XmlElement("sound")] public Sound? Sound { get; set; }
    public bool ShouldSerializeSound() => Sound != null && !IsEs;

    [XmlElement("input")] public Input? Input { get; set; }
    public bool ShouldSerializeInput() => Input != null && !IsEs;

    [XmlElement("dipswitch")] public List<DipSwitch>? DipSwitches { get; set; }
    public bool ShouldSerializeDipSwitches() => DipSwitches != null && !IsEs;
}

[Serializable]
public class Rom
{
    [XmlAttribute("name")] public string? Name { get; set; }
    [XmlAttribute("merge")] public string? Merge { get; set; }
    [XmlAttribute("size")] public string? Size { get; set; }
    [XmlAttribute("crc")] public string? Crc { get; set; }
    [XmlAttribute("sha1")] public string? Sha1 { get; set; }
    [XmlAttribute("status")] public string? Status { get; set; }
    [XmlAttribute("bios")] public string? Bios { get; set; } // mame2003
    [XmlAttribute("md5")] public string? Md5 { get; set; }
    [XmlAttribute("region")] public string? Region { get; set; }
    [XmlAttribute("offset")] public string? Offset { get; set; }
    [XmlAttribute("dispose")] public string? Dispose { get; set; }
    [XmlAttribute("soundonly")] public string? SoundOnly { get; set; }
}

[Serializable]
public class BiosSet
{
    [XmlAttribute("name")] public string? Name { get; set; }
    [XmlAttribute("description")] public string? Description { get; set; }
    [XmlAttribute("default")] public string? Default { get; set; } // mame2003
}

[Serializable]
public class DeviceRef
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[Serializable]
public class SoftwareList
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[Serializable]
public class Sample
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[Serializable]
public class Driver
{
    [XmlAttribute("status")] public string? Status { get; set; }
    [XmlAttribute("color")] public string? Color { get; set; } // mame2003
    [XmlAttribute("sound")] public string? Sound { get; set; }
    [XmlAttribute("palettesize")] public string? PaletteSize { get; set; }
}

// mame2003
[Serializable]
public class Disk
{
    [XmlAttribute("name")] public string? Name { get; set; }
    [XmlAttribute("md5")] public string? Md5 { get; set; }
    [XmlAttribute("sha1")] public string? Sha1 { get; set; }
    [XmlAttribute("region")] public string? Region { get; set; }
    [XmlAttribute("index")] public string? Index { get; set; }
}

// mame2003
[Serializable]
public class Chip
{
    [XmlAttribute("name")] public string? Name { get; set; }
    [XmlAttribute("type")] public string? Type { get; set; }
    [XmlAttribute("soundonly")] public string? SoundOnly { get; set; }
    [XmlAttribute("clock")] public string? Clock { get; set; }
}

// mame2003
[Serializable]
public class Video
{
    [XmlAttribute("screen")] public string? Screen { get; set; }
    [XmlAttribute("type")] public string? Type { get; set; }
    [XmlAttribute("orientation")] public string? Orientation { get; set; }
    [XmlAttribute("width")] public string? Width { get; set; }
    [XmlAttribute("height")] public string? Height { get; set; }
    [XmlAttribute("aspectx")] public string? AspectX { get; set; }
    [XmlAttribute("aspecty")] public string? AspectY { get; set; }
    [XmlAttribute("refresh")] public string? Refresh { get; set; }
}

// mame2003
[Serializable]
public class Sound
{
    [XmlAttribute("channels")] public string? Channels { get; set; }
}

// mame2003
[Serializable]
public class Input
{
    [XmlAttribute("service")] public string? Service { get; set; }
    [XmlAttribute("tilt")] public string? Tilt { get; set; }
    [XmlAttribute("players")] public string? Players { get; set; }
    [XmlAttribute("control")] public string? Control { get; set; }
    [XmlAttribute("buttons")] public string? Buttons { get; set; }
    [XmlAttribute("coins")] public string? Coins { get; set; }
}

// mame2003
[Serializable]
public class DipValue
{
    [XmlAttribute("Name")] public string? Name { get; set; }
    [XmlAttribute("default")] public string? Default { get; set; }
}

// mame2003
[Serializable]
public class DipSwitch
{
    [XmlAttribute("name")] public string? Name { get; set; }
    [XmlElement("dipvalue")] public List<DipValue>? DipValues { get; set; }
}