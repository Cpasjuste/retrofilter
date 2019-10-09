using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
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
                reader.Close();
                if (dataFile.Games.Count <= 0)
                {
                    Console.WriteLine("Could not parse gamelist: no games found");
                    return false;
                }

                // parse embedded catver
                // TODO: too slow
                string catver = GetEmbeddedResource("catver.ini");
                List<string> catverLines = new List<string>(catver.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                foreach (Game game in dataFile.Games)
                {
                    string genre = catverLines.Find(x => x.StartsWith(game.Name + "="));
                    // if genre was not found, try to find parent genre
                    if (string.IsNullOrEmpty(genre) && game.IsClone)
                    {
                        genre = catverLines.Find(x => x.StartsWith(game.CloneOf + "="));
                    }
                    if (!string.IsNullOrEmpty(genre))
                    {
                        game.Genre = genre.Split('=')[1];
                    }
                }

                // finally, create ObservableCollection for datagrid
                dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Games);
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

        private string GetEmbeddedResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            resourceName = assembly.GetName().Name + "." +
                resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
