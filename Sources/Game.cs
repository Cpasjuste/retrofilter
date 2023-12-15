using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RetroFilter.Sources;

// xml root: datafile, xml elements names: "machine" (Mame)
[System.Serializable]
public class Machine : Game
{
}

// xml root: datafile, xml elements names: "game" (Mame)
[System.Serializable]
public class Game
{
    //[XmlIgnore]
    //public bool Locked { get; set; }

    // ES
    [XmlAttribute(AttributeName = "id")]
    public string? Id { get; set; }

    [XmlAttribute(AttributeName = "source")]
    public string? Source { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    // ES
    [XmlElement(ElementName = "name")]
    public string? NameEs { get; set; }

    [XmlElement(ElementName = "description")]
    public string? Description { get; set; }

    [DefaultValue("")]
    [XmlElement(ElementName = "comment")]
    public string? Comment { get; set; }

    // ES
    [XmlElement(ElementName = "path")]
    public string? Path { get; set; }

    [XmlAttribute(AttributeName = "isbios")]
    public string? IsBios { get; set; }

    [XmlAttribute(AttributeName = "isdevice")]
    private string? IsDevice { get; set; }

    [XmlAttribute(AttributeName = "runnable")]
    public string? Runnable { get; set; }

    // ES
    [XmlElement(ElementName = "desc")]
    public string? Desc { get; set; }

    // ES
    [XmlElement(ElementName = "rating")]
    public string? Rating { get; set; }

    [XmlElement(ElementName = "year")]
    public string? Year { get; set; }

    // ES
    [XmlElement(ElementName = "releasedate")]
    public string? ReleaseDate { get; set; }

    [XmlElement(ElementName = "manufacturer")]
    public string? Manufacturer { get; set; }

    // ES
    [XmlElement(ElementName = "developer")]
    public string? Developer { get; set; }

    [XmlElement(ElementName = "publisher")]
    public string? Publisher { get; set; }

    [XmlElement(ElementName = "genre")]
    public string? Genre { get; set; }

    // ES
    [XmlElement(ElementName = "players")]
    public string? Players { get; set; }

    [XmlElement(ElementName = "region")]
    public string? Region { get; set; }

    [XmlAttribute(AttributeName = "romof")]
    public string? RomOf { get; set; }

    [XmlAttribute(AttributeName = "cloneof")]
    public string? CloneOf { get; set; }

    // mame2003
    [XmlAttribute(AttributeName = "sampleof")]
    public string? SampleOf { get; set; }

    [XmlIgnore]
    public string IsClone => string.IsNullOrEmpty(CloneOf) ? string.Empty : "yes";

    [XmlAttribute(AttributeName = "sourcefile")]
    public string? SourceFile { get; set; }

    // ES
    [XmlElement(ElementName = "romtype")]
    public string? RomType { get; set; }

    [XmlElement(ElementName = "hash")]
    public string? Hash { get; set; }

    [XmlElement(ElementName = "image")]
    public string? Image { get; set; }

    [XmlElement(ElementName = "video")]
    public string? Video { get; set; }

    [XmlElement(ElementName = "thumbnail")]
    public string? Thumbnail { get; set; }

    [XmlElement(ElementName = "biosset")]
    public List<BiosSet>? BiosSets { get; set; }

    [XmlElement(ElementName = "rom")]
    public List<Rom>? Roms { get; set; }

    [XmlElement(ElementName = "device_ref")]
    public List<DeviceRef>? DeviceRefs { get; set; }

    [XmlElement(ElementName = "sample")]
    public List<Sample>? Samples { get; set; }

    [XmlElement(ElementName = "driver")]
    public Driver? Driver { get; set; }

    [XmlIgnore]
    public string? Status => Driver?.Status;

    // mame2003
    [XmlElement(ElementName = "disk")]
    public List<Disk>? Disks { get; set; }

    [XmlElement(ElementName = "chip")]
    public List<Chip>? Chips { get; set; }

    //[XmlElement]
    //public Video? Video { get; set; }

    [XmlElement(ElementName = "sound")]
    public Sound? Sound { get; set; }

    [XmlElement(ElementName = "input")]
    public Input? Input { get; set; }

    [XmlElement(ElementName = "dipswitch")]
    public List<DipSwitch>? DipSwitches { get; set; }
}

[System.Serializable]
public class Rom
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "merge")]
    public string? Merge { get; set; }

    [XmlAttribute(AttributeName = "size")]
    public string? Size { get; set; }

    [XmlAttribute(AttributeName = "crc")]
    public string? Crc { get; set; }

    [XmlAttribute(AttributeName = "sha1")]
    public string? Sha1 { get; set; }

    [XmlAttribute(AttributeName = "status")]
    public string? Status { get; set; }

    // mame2003
    [XmlAttribute(AttributeName = "bios")]
    public string? Bios { get; set; }

    [XmlAttribute(AttributeName = "md5")]
    public string? Md5 { get; set; }

    [XmlAttribute(AttributeName = "region")]
    public string? Region { get; set; }

    [XmlAttribute(AttributeName = "offset")]
    public string? Offset { get; set; }

    [XmlAttribute(AttributeName = "dispose")]
    public string? Dispose { get; set; }

    [XmlAttribute(AttributeName = "soundonly")]
    public string? SoundOnly { get; set; }
}

[System.Serializable]
public class BiosSet
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "description")]
    public string? Description { get; set; }

    // mame2003
    [XmlAttribute(AttributeName = "default")]
    public string? Default { get; set; }
}

[System.Serializable]
public class DeviceRef
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }
}

[System.Serializable]
public class Sample
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }
}

[System.Serializable]
public class Driver
{
    [XmlAttribute(AttributeName = "status")]
    public string? Status { get; set; }

    // mame2003
    [XmlAttribute(AttributeName = "color")]
    public string? Color { get; set; }

    [XmlAttribute(AttributeName = "sound")]
    public string? Sound { get; set; }

    [XmlAttribute(AttributeName = "palettesize")]
    public string? PaletteSize { get; set; }
}

// mame2003
[System.Serializable]
public class Disk
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "md5")]
    public string? Md5 { get; set; }

    [XmlAttribute(AttributeName = "sha1")]
    public string? Sha1 { get; set; }

    [XmlAttribute(AttributeName = "region")]
    public string? Region { get; set; }

    [XmlAttribute(AttributeName = "index")]
    public string? Index { get; set; }
}

// mame2003
[System.Serializable]
public class Chip
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string? Type { get; set; }

    [XmlAttribute(AttributeName = "soundonly")]
    public string? SoundOnly { get; set; }

    [XmlAttribute(AttributeName = "clock")]
    public string? Clock { get; set; }
}

// mame2003
[System.Serializable]
public class Video
{
    [XmlAttribute(AttributeName = "screen")]
    public string? Screen { get; set; }

    [XmlAttribute(AttributeName = "orientation")]
    public string? Orientation { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public string? Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public string? Height { get; set; }

    [XmlAttribute(AttributeName = "aspectx")]
    public string? AspectX { get; set; }

    [XmlAttribute(AttributeName = "aspecty")]
    public string? AspectY { get; set; }

    [XmlAttribute(AttributeName = "refresh")]
    public string? Refresh { get; set; }
}

// mame2003
[System.Serializable]
public class Sound
{
    [XmlAttribute(AttributeName = "channels")]
    public string? Channels { get; set; }
}

// mame2003
[System.Serializable]
public class Input
{
    [XmlAttribute(AttributeName = "service")]
    public string? Service { get; set; }

    [XmlAttribute(AttributeName = "tilt")]
    public string? Tilt { get; set; }

    [XmlAttribute(AttributeName = "players")]
    public string? Players { get; set; }

    [XmlAttribute(AttributeName = "control")]
    public string? Control { get; set; }

    [XmlAttribute(AttributeName = "buttons")]
    public string? Buttons { get; set; }

    [XmlAttribute(AttributeName = "coins")]
    public string? Coins { get; set; }
}

// mame2003
[System.Serializable]
public class DipValue
{
    [XmlAttribute(AttributeName = "Name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "default")]
    public string? Default { get; set; }
}

// mame2003
[System.Serializable]
public class DipSwitch
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "dipvalue")]
    public List<DipValue>? DipValues { get; set; }
}