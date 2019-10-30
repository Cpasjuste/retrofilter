
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RetroFilter
{
    // xml root: datafile, xml elements names: "machine" (Mame)
    [System.Serializable()]
    public class Machine : Game { }

    // xml root: datafile, xml elements names: "game" (Mame)
    [System.Serializable()]
    public class Game
    {
        [XmlIgnore]
        public bool Locked { get; set; }

        // ES
        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string source { get; set; }

        [XmlAttribute]
        public string name { get; set; }

        // ES
        [XmlElement(ElementName = "name")]
        public string nameES { get; set; }

        // ES
        [XmlElement]
        public string path { get; set; }

        [XmlAttribute(AttributeName = "isbios")]
        public string isbios_ { get; set; }
        [XmlIgnore]
        public bool isbios { get { return isbios_ == "yes"; } set { } }

        [XmlAttribute(AttributeName = "isdevice")]
        public string isdevice_ { get; set; }
        [XmlIgnore]
        public bool isdevice { get { return isdevice_ == "yes"; } set { } }

        [XmlAttribute(AttributeName = "runnable")]
        public string runnable_ { get; set; }
        public bool runnable { get { return runnable_ != "no"; } set { } }

        [XmlElement]
        public string description { get; set; }

        // ES
        [XmlElement]
        public string desc { get; set; }

        // ES
        [XmlElement]
        public string rating { get; set; }

        [XmlElement]
        public string year { get; set; }

        // ES
        [XmlElement]
        public string releasedate { get; set; }

        [XmlElement]
        public string manufacturer { get; set; }

        // ES
        [XmlElement]
        public string developer { get; set; }
        [XmlElement]
        public string publisher { get; set; }
        [XmlElement]
        public string genre { get; set; }

        // ES
        [XmlElement]
        public string players { get; set; }
        [XmlElement]
        public string region { get; set; }

        [XmlAttribute]
        public string romof { get; set; }
        [XmlAttribute]
        public string cloneof { get; set; }
        // mame2003
        [XmlAttribute]
        public string sampleof { get; set; }

        [XmlIgnore]
        public bool isclone { get { return !string.IsNullOrEmpty(cloneof); } set { } }

        [XmlAttribute]
        public string sourcefile { get; set; }

        // ES
        [XmlElement]
        public string romtype { get; set; }
        [XmlElement]
        public string hash { get; set; }
        [XmlElement]
        public string image { get; set; }
        [XmlElement]
        public string thumbnail { get; set; }

        [XmlElement]
        public List<biosset> biosset { get; set; }
        [XmlElement]
        public List<rom> rom { get; set; }
        [XmlElement]
        public List<device_ref> device_ref { get; set; }
        [XmlElement]
        public List<sample> sample { get; set; }
        [XmlElement]
        public driver driver { get; set; }

        // mame2003
        [XmlElement]
        public List<disk> disk { get; set; }
        [XmlElement]
        public List<chip> chip { get; set; }
        [XmlElement]
        public video video { get; set; }
        [XmlElement]
        public sound sound { get; set; }
        [XmlElement]
        public input input { get; set; }
        [XmlElement]
        public List<dipswitch> dipswitch { get; set; }
    }

    [System.Serializable()]
    public class rom
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string merge { get; set; }
        [XmlAttribute]
        public string size { get; set; }
        [XmlAttribute]
        public string crc { get; set; }
        [XmlAttribute]
        public string sha1 { get; set; }
        [XmlAttribute]
        public string status { get; set; }

        // mame2003
        [XmlAttribute]
        public string bios { get; set; }
        [XmlAttribute]
        public string md5 { get; set; }
        [XmlAttribute]
        public string region { get; set; }
        [XmlAttribute]
        public string offset { get; set; }
        [XmlAttribute]
        public string dispose { get; set; }
        [XmlAttribute]
        public string soundonly { get; set; }
    }

    [System.Serializable()]
    public class biosset
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string description { get; set; }
        // mame2003
        [XmlAttribute(AttributeName = "default")]
        public string default_ { get; set; }
    }

    [System.Serializable()]
    public class device_ref
    {
        [XmlAttribute]
        public string name { get; set; }
    }

    [System.Serializable()]
    public class sample
    {
        [XmlAttribute]
        public string name { get; set; }
    }

    [System.Serializable()]
    public class driver
    {
        [XmlAttribute]
        public string status { get; set; }
        // mame2003
        [XmlAttribute]
        public string color { get; set; }
        [XmlAttribute]
        public string sound { get; set; }
        [XmlAttribute]
        public string palettesize { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class disk
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string md5 { get; set; }
        [XmlAttribute]
        public string sha1 { get; set; }
        [XmlAttribute]
        public string region { get; set; }
        [XmlAttribute]
        public string index { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class chip
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string soundonly { get; set; }
        [XmlAttribute]
        public string clock { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class video
    {
        [XmlAttribute]
        public string screen { get; set; }
        [XmlAttribute]
        public string orientation { get; set; }
        [XmlAttribute]
        public string width { get; set; }
        [XmlAttribute]
        public string height { get; set; }
        [XmlAttribute]
        public string aspectx { get; set; }
        [XmlAttribute]
        public string aspecty { get; set; }
        [XmlAttribute]
        public string refresh { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class sound
    {
        [XmlAttribute]
        public string channels { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class input
    {
        [XmlAttribute]
        public string service { get; set; }
        [XmlAttribute]
        public string tilt { get; set; }
        [XmlAttribute]
        public string players { get; set; }
        [XmlAttribute]
        public string control { get; set; }
        [XmlAttribute]
        public string buttons { get; set; }
        [XmlAttribute]
        public string coins { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class dipvalue
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute(AttributeName = "default")]
        public string default_ { get; set; }
    }

    // mame2003
    [System.Serializable()]
    public class dipswitch
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlElement]
        public List<dipvalue> dipvalue { get; set; }
    }
}
