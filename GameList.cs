using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RetroFilter
{
    public class GameList
    {
        public DataFile dataFile;

        [Serializable()]
        [XmlRoot("datafile", Namespace = "", IsNullable = false)]
        public class DataFile
        {
            [XmlElement(ElementName = "game")]
            public List<Game> Game { get; set; }
            [XmlIgnore]
            public ObservableCollection<Game> gamesCollection { get; set; }
        }

        public bool Load(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                StreamReader reader = new StreamReader(path);
                dataFile = (DataFile)serializer.Deserialize(reader);
                dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Game);
                reader.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }

            return true;
        }

        public bool Save(string path)
        {
            try
            {
                TextWriter writer = new StreamWriter(path);
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                serializer.Serialize(writer, dataFile);
                writer.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }
            return true;
        }
    }
}
