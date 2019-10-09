using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace RetroFilter
{
    [Serializable()]
    [XmlRoot("datafile", Namespace = "", IsNullable = false)]
    public class DataFile
    {
        [XmlElement("game", typeof(Game))]
        [XmlElement("machine", typeof(MameGame))]
        public List<Game> Games { get; set; }
        [XmlIgnore]
        public ObservableCollection<Game> gamesCollection { get; set; }
    }
}
