using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }

            if (dataFile.Games.Count <= 0)
            {
                Console.WriteLine("Could not parse gamelist: no games found");
                return false;
            }

            // parse embedded catver, convert it to dictionnary for faster search
            string catver = GetEmbeddedResource("catver_0.214.ini");
            string[] catverLines = catver.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> catverDic = catverLines.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
            foreach (Game game in dataFile.Games)
            {
                if (!catverDic.TryGetValue(game.Name, out string genre) && game.IsClone)
                {
                    catverDic.TryGetValue(game.CloneOf, out genre);
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    game.Genre = genre;
                }
            }

            // finally, create ObservableCollection for datagrid
            dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Games);

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
