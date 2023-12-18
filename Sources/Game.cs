using System.Collections.Generic;
using System.Xml.Serialization;
using RetroFilter.Controls;

namespace RetroFilter.Sources;

// xml root: datafile, xml elements names: "machine" (Mame)
[System.Serializable]
public class Machine : Game
{
}

// xml root: datafile, xml elements names: "game" (Mame/Es)
[System.Serializable]
public class Game
{
    private static bool IsEs => MainWindow.OutputType == DataFile.Type.EmulationStation;

    //[XmlIgnore]
    //public bool Locked { get; set; }

    // ES
    [XmlAttribute("id")] public string? Id { get; set; }
    public bool ShouldSerializeId() => Id != null && IsEs;

    [XmlAttribute("source")] public string? Source { get; set; }
    public bool ShouldSerializeSource() => Source != null && IsEs;

    [XmlAttribute("name")] public string? Name { get; set; }
    public bool ShouldSerializeName() => Name != null && !IsEs;

    // ES
    [XmlElement("name")] public string? NameEs { get; set; }
    public bool ShouldSerializeNameEs() => NameEs != null && IsEs;

    [XmlElement("description")] public string? Description { get; set; }
    public bool ShouldSerializeDescription() => Description != null && !IsEs;

    [XmlElement("comment")] public string? Comment { get; set; }
    public bool ShouldSerializeComment() => Comment != null && !IsEs;

    // ES
    [XmlElement("path")] public string? Path { get; set; }
    public bool ShouldSerializePath() => Path != null && IsEs;

    [XmlAttribute("isbios")] public string? IsBios { get; set; }
    public bool ShouldSerializeIsBios() => IsBios != null && !IsEs;

    [XmlAttribute("isdevice")] private string? IsDevice { get; set; }
    public bool ShouldSerializeIsDevice() => IsDevice != null && !IsEs;

    [XmlAttribute("ismechanical")] private string? IsMechanical { get; set; }
    public bool ShouldSerializeIsMechanical() => IsMechanical != null && !IsEs;

    [XmlAttribute("runnable")] public string? Runnable { get; set; }
    public bool ShouldSerializeRunnable() => Runnable != null && !IsEs;

    // ES
    [XmlElement("desc")] public string? Desc { get; set; }
    public bool ShouldSerializeDesc() => Desc != null && IsEs;

    // ES
    [XmlElement("rating")] public string? Rating { get; set; }
    public bool ShouldSerializeRating() => Rating != null && IsEs;

    [XmlElement("year")] public string? Year { get; set; }
    public bool ShouldSerializeYear() => Year != null && !IsEs;

    // ES
    [XmlElement("releasedate")] public string? ReleaseDate { get; set; }
    public bool ShouldSerializeReleaseDate() => ReleaseDate != null && IsEs;

    [XmlElement("manufacturer")] public string? Manufacturer { get; set; }
    public bool ShouldSerializeManufacturer() => Manufacturer != null && !IsEs;

    // ES
    [XmlElement("developer")] public string? Developer { get; set; }
    public bool ShouldSerializeDeveloper() => Developer != null && IsEs;

    [XmlElement("publisher")] public string? Publisher { get; set; }
    public bool ShouldSerializePublisher() => Publisher != null && IsEs;

    // ES
    [XmlElement("system")] public string? System { get; set; }
    public bool ShouldSerializeSystem() => System != null && IsEs;

    [XmlElement("genre")] public string? Genre { get; set; }
    public bool ShouldSerializeGenre() => Genre != null && IsEs;

    // ES
    [XmlElement("genreid")] public string? GenreId { get; set; }
    public bool ShouldSerializeGenreId() => GenreId != null && IsEs;

    [XmlElement("resolution")] public string? Resolution { get; set; }
    public bool ShouldSerializeResolution() => Resolution != null && IsEs;

    [XmlElement("rotation")] public string? Rotation { get; set; }
    public bool ShouldSerializeRotation() => Rotation != null && IsEs;

    // ES
    [XmlElement("players")] public string? Players { get; set; }
    public bool ShouldSerializePlayers() => Players != null && IsEs;

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

    [XmlElement("hash")] public string? Hash { get; set; }
    public bool ShouldSerializeHash() => Hash != null && IsEs;

    [XmlElement("image")] public string? Image { get; set; }
    public bool ShouldSerializeImage() => Image != null && IsEs;

    [XmlElement("video")] public string? Video { get; set; }
    public bool ShouldSerializeVideo() => Video != null && IsEs;

    [XmlElement("thumbnail")] public string? Thumbnail { get; set; }
    public bool ShouldSerializeThumbnail() => Thumbnail != null && IsEs;

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

    //[XmlElement]
    //public Video? Video { get; set; }

    [XmlElement("sound")] public Sound? Sound { get; set; }
    public bool ShouldSerializeSound() => Sound != null && !IsEs;

    [XmlElement("input")] public Input? Input { get; set; }
    public bool ShouldSerializeInput() => Input != null && !IsEs;

    [XmlElement("dipswitch")] public List<DipSwitch>? DipSwitches { get; set; }
    public bool ShouldSerializeDipSwitches() => DipSwitches != null && !IsEs;
}

[System.Serializable]
public class Rom
{
    [XmlAttribute("name")] public string? Name { get; set; }

    [XmlAttribute("merge")] public string? Merge { get; set; }

    [XmlAttribute("size")] public string? Size { get; set; }

    [XmlAttribute("crc")] public string? Crc { get; set; }

    [XmlAttribute("sha1")] public string? Sha1 { get; set; }

    [XmlAttribute("status")] public string? Status { get; set; }

    // mame2003
    [XmlAttribute("bios")] public string? Bios { get; set; }

    [XmlAttribute("md5")] public string? Md5 { get; set; }

    [XmlAttribute("region")] public string? Region { get; set; }

    [XmlAttribute("offset")] public string? Offset { get; set; }

    [XmlAttribute("dispose")] public string? Dispose { get; set; }

    [XmlAttribute("soundonly")] public string? SoundOnly { get; set; }
}

[System.Serializable]
public class BiosSet
{
    [XmlAttribute("name")] public string? Name { get; set; }

    [XmlAttribute("description")] public string? Description { get; set; }

    // mame2003
    [XmlAttribute("default")] public string? Default { get; set; }
}

[System.Serializable]
public class DeviceRef
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[System.Serializable]
public class SoftwareList
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[System.Serializable]
public class Sample
{
    [XmlAttribute("name")] public string? Name { get; set; }
}

[System.Serializable]
public class Driver
{
    [XmlAttribute("status")] public string? Status { get; set; }

    // mame2003
    [XmlAttribute("color")] public string? Color { get; set; }

    [XmlAttribute("sound")] public string? Sound { get; set; }

    [XmlAttribute("palettesize")] public string? PaletteSize { get; set; }
}

// mame2003
[System.Serializable]
public class Disk
{
    [XmlAttribute("name")] public string? Name { get; set; }

    [XmlAttribute("md5")] public string? Md5 { get; set; }

    [XmlAttribute("sha1")] public string? Sha1 { get; set; }

    [XmlAttribute("region")] public string? Region { get; set; }

    [XmlAttribute("index")] public string? Index { get; set; }
}

// mame2003
[System.Serializable]
public class Chip
{
    [XmlAttribute("name")] public string? Name { get; set; }

    [XmlAttribute("type")] public string? Type { get; set; }

    [XmlAttribute("soundonly")] public string? SoundOnly { get; set; }

    [XmlAttribute("clock")] public string? Clock { get; set; }
}

// mame2003
[System.Serializable]
public class Video
{
    [XmlAttribute("screen")] public string? Screen { get; set; }

    [XmlAttribute("orientation")] public string? Orientation { get; set; }

    [XmlAttribute("width")] public string? Width { get; set; }

    [XmlAttribute("height")] public string? Height { get; set; }

    [XmlAttribute("aspectx")] public string? AspectX { get; set; }

    [XmlAttribute("aspecty")] public string? AspectY { get; set; }

    [XmlAttribute("refresh")] public string? Refresh { get; set; }
}

// mame2003
[System.Serializable]
public class Sound
{
    [XmlAttribute("channels")] public string? Channels { get; set; }
}

// mame2003
[System.Serializable]
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
[System.Serializable]
public class DipValue
{
    [XmlAttribute("Name")] public string? Name { get; set; }

    [XmlAttribute("default")] public string? Default { get; set; }
}

// mame2003
[System.Serializable]
public class DipSwitch
{
    [XmlAttribute("name")] public string? Name { get; set; }

    [XmlElement("dipvalue")] public List<DipValue>? DipValues { get; set; }
}