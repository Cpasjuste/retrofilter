using System;
using System.Xml.Serialization;

namespace RetroFilter
{
    [Serializable()]
    public class Header
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "category")]
        public string Category { get; set; }

        [XmlElement(ElementName = "version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "author")]
        public string Author { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "homepage")]
        public string Homepage { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "comment")]
        public string Comment { get; set; }

        [XmlElement(ElementName = "clrmamepro")]
        public Clrmamepro Clrmamepro { get; set; }
    }

    [Serializable()]
    public class Clrmamepro
    {
        [XmlAttribute(AttributeName = "forcenodump")]
        public string ForceNoDump { get; set; }
    }
}
