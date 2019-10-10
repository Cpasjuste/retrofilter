
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RetroFilter
{
    // xml elements names: "machine"
    [System.Serializable()]
    public class MameGame : Game { }

    // xml elements names: "game" (libretro dats, emulationstation dats...)
    [System.Serializable()]
    public class Game
    {
        [XmlIgnore]
        public bool Locked { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "sourcefile")]
        public string SourceFile { get; set; }

        [XmlAttribute(AttributeName = "isbios")]
        public string IsBiosInternal { get; set; }
        [XmlIgnore]
        public bool IsBios { get { return IsBiosInternal == "yes"; } set { } }

        [XmlAttribute(AttributeName = "isdevice")]
        public string IsDevice { get; set; }

        [XmlAttribute(AttributeName = "runnable")]
        public string Runnable { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "year")]
        public string Year { get; set; }

        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; }

        [XmlElement(ElementName = "genre")]
        public string Genre { get; set; }

        [XmlAttribute(AttributeName = "romof")]
        public string RomOf { get; set; }

        [XmlAttribute(AttributeName = "cloneof")]
        public string CloneOf { get; set; }

        [XmlIgnore]
        public bool IsClone { get { return !string.IsNullOrEmpty(CloneOf); } set { } }

        [XmlElement(ElementName = "biosset")]
        public List<BiosSet> BiosSets { get; set; }

        [XmlElement(ElementName = "rom")]
        public List<Rom> Roms { get; set; }

        [XmlElement(ElementName = "device_ref")]
        public List<DeviceRef> DeviceRefs { get; set; }

        [XmlElement(ElementName = "sample")]
        public List<Sample> Samples { get; set; }

        [XmlElement(ElementName = "driver")]
        public Driver Driver { get; set; }
    }

    [System.Serializable()]
    public class Rom
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "merge")]
        public string Merge { get; set; }

        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }

        [XmlAttribute(AttributeName = "crc")]
        public string Crc { get; set; }

        [XmlAttribute(AttributeName = "sha1")]
        public string Sha1 { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

    [System.Serializable()]
    public class BiosSet
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
    }

    [System.Serializable()]
    public class DeviceRef
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [System.Serializable()]
    public class Sample
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [System.Serializable()]
    public class Driver
    {
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }
}
