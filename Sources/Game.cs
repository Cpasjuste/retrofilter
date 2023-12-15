using System.Collections.Generic;
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
    public string Id { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "name")]
    public string NameEs { get; set; } = string.Empty;

    [XmlElement(ElementName = "description")]
    public string Description { get; set; } = string.Empty;

    [XmlElement(ElementName = "comment")]
    public string Comment { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "path")]
    public string Path { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "isbios")]
    public string IsBios { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "isdevice")]
    private string IsDevice { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "runnable")]
    public string Runnable { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "desc")]
    public string Desc { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "rating")]
    public string Rating { get; set; } = string.Empty;

    [XmlElement(ElementName = "year")]
    public string Year { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "releasedate")]
    public string ReleaseDate { get; set; } = string.Empty;

    [XmlElement(ElementName = "manufacturer")]
    public string Manufacturer { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "developer")]
    public string Developer { get; set; } = string.Empty;

    [XmlElement(ElementName = "publisher")]
    public string Publisher { get; set; } = string.Empty;

    [XmlElement(ElementName = "genre")]
    public string Genre { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "players")]
    public string Players { get; set; } = string.Empty;

    [XmlElement(ElementName = "region")]
    public string Region { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "romof")]
    public string RomOf { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "cloneof")]
    public string CloneOf { get; set; } = string.Empty;

    // mame2003
    [XmlAttribute(AttributeName = "sampleof")]
    public string SampleOf { get; set; } = string.Empty;

    [XmlIgnore]
    public string IsClone => string.IsNullOrEmpty(CloneOf) ? string.Empty : "yes";

    [XmlAttribute(AttributeName = "sourcefile")]
    public string SourceFile { get; set; } = string.Empty;

    // ES
    [XmlElement(ElementName = "romtype")]
    public string RomType { get; set; } = string.Empty;

    [XmlElement(ElementName = "hash")]
    public string Hash { get; set; } = string.Empty;

    [XmlElement(ElementName = "image")]
    public string Image { get; set; } = string.Empty;

    [XmlElement(ElementName = "video")]
    public string Video { get; set; } = string.Empty;

    [XmlElement(ElementName = "thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;

    [XmlElement(ElementName = "biosset")]
    public List<BiosSet> BiosSets { get; set; } = new();

    [XmlElement(ElementName = "rom")]
    public List<Rom> Roms { get; set; } = new();

    [XmlElement(ElementName = "device_ref")]
    public List<DeviceRef> DeviceRefs { get; set; } = new();

    [XmlElement(ElementName = "sample")]
    public List<Sample> Samples { get; set; } = new();

    [XmlElement(ElementName = "driver")]
    public Driver Driver { get; set; } = new();

    [XmlIgnore]
    public string Status => Driver.Status;

    // mame2003
    [XmlElement(ElementName = "disk")]
    public List<Disk> Disks { get; set; } = new();

    [XmlElement(ElementName = "chip")]
    public List<Chip> Chips { get; set; } = new();

    //[XmlElement]
    //public video video { get; set; }

    [XmlElement(ElementName = "sound")]
    public Sound Sound { get; set; } = new();

    [XmlElement(ElementName = "input")]
    public Input Input { get; set; } = new();

    [XmlElement(ElementName = "dipswitch")]
    public List<DipSwitch> DipSwitches { get; set; } = new();
}

[System.Serializable]
public class Rom
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "merge")]
    public string Merge { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "size")]
    public string Size { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "crc")]
    public string Crc { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "sha1")]
    public string Sha1 { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "status")]
    public string Status { get; set; } = string.Empty;

    // mame2003
    [XmlAttribute(AttributeName = "bios")]
    public string Bios { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "md5")]
    public string Md5 { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "region")]
    public string Region { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "offset")]
    public string Offset { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "dispose")]
    public string Dispose { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "soundonly")]
    public string SoundOnly { get; set; } = string.Empty;
}

[System.Serializable]
public class BiosSet
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "description")]
    public string Description { get; set; } = string.Empty;

    // mame2003
    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; } = string.Empty;
}

[System.Serializable]
public class DeviceRef
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;
}

[System.Serializable]
public class Sample
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;
}

[System.Serializable]
public class Driver
{
    [XmlAttribute(AttributeName = "status")]
    public string Status { get; set; } = string.Empty;

    // mame2003
    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "sound")]
    public string Sound { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "palettesize")]
    public string PaletteSize { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class Disk
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "md5")]
    public string Md5 { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "sha1")]
    public string Sha1 { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "region")]
    public string Region { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "index")]
    public string Index { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class Chip
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "soundonly")]
    public string SoundOnly { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "clock")]
    public string Clock { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class Video
{
    [XmlAttribute(AttributeName = "screen")]
    public string Screen { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "orientation")]
    public string Orientation { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "aspectx")]
    public string AspectX { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "aspecty")]
    public string AspectY { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "refresh")]
    public string Refresh { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class Sound
{
    [XmlAttribute(AttributeName = "channels")]
    public string Channels { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class Input
{
    [XmlAttribute(AttributeName = "service")]
    public string Service { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "tilt")]
    public string Tilt { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "players")]
    public string Players { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "control")]
    public string Control { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "buttons")]
    public string Buttons { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "coins")]
    public string Coins { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class DipValue
{
    [XmlAttribute(AttributeName = "Name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; } = string.Empty;
}

// mame2003
[System.Serializable]
public class DipSwitch
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement(ElementName = "dipvalue")]
    public List<DipValue> DipValues { get; set; } = new();
}