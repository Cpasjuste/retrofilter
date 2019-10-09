using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace RetroFilter
{
    public class GameList
    {
        public DataFile dataFile;

        public bool Load(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                StreamReader reader = new StreamReader(path);
                dataFile = (DataFile)serializer.Deserialize(reader);
                dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Games);
                reader.Close();
                if (dataFile.Games.Count <= 0)
                {
                    Console.WriteLine("Could not parse gamelist: no games found");
                    return false;
                }
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
                dataFile.Games = new List<Game>(dataFile.gamesCollection);
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
