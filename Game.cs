
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RetroFilter
{
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
    }

    [System.Serializable()]
    public class Driver
    {
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

    [System.Serializable()]
    public class Game
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "year")]
        public string Year { get; set; }

        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute(AttributeName = "romof")]
        public string RomOf { get; set; }

        [XmlAttribute(AttributeName = "cloneof")]
        public string CloneOf { get; set; }

        [XmlIgnore]
        public bool IsClone { get; set; }

        [XmlElement(ElementName = "rom")]
        public List<Rom> Rom { get; set; }
    }
}
