using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace RetroFilter
{
    [Serializable()]
    [XmlRoot("datafile", Namespace = "", IsNullable = false)]
    public class DataFile
    {
        public enum Type
        {
            MameGame,
            MameMachine,
            EmulationStation
        };

        [XmlElement(ElementName = "header")]
        public Header Header { get; set; }
        [XmlElement("game", typeof(Game))]
        [XmlElement("machine", typeof(Machine))]
        public List<Game> Games { get; set; }
        [XmlIgnore]
        public ObservableCollection<Game> gamesCollection { get; set; }
        [XmlIgnore]
        public Type type = Type.MameGame;

        public List<string> GetFilteredColumns()
        {
            List<string> filters = new List<string>();

            filters.AddRange(new List<string>() {
                "IsBiosInternal","Driver" ,"BiosSets"
                ,"Roms" ,"DeviceRefs","Samples","IsDeleteEnabled"
            });

            if (type == Type.MameGame || type == Type.MameMachine)
            {
                filters.AddRange(new List<string>()
                {  "Desc", "Source", "Image", "Thumbnail", "ReleaseDate",
                    "Path", "Developer", "Region", "RomType", "Id", "Publisher"});
            }

            if (type == Type.MameGame)
            {
                filters.AddRange(new List<string>()
                {  "SourceFile", "IsDevice", "Runnable" });
            }
            else if (type == Type.MameMachine)
            {

            }
            else if (type == Type.EmulationStation)
            {
                filters.AddRange(new List<string>()
                {  "SourceFile", "IsDevice", "Runnable",
                    "IsBios", "Year", "Description",
                    "Manufacturer", "RomOf", "CloneOf", "IsRom", "IsClone" });
            }

            return filters;
        }

        public static DataFile Load(string path)
        {
            DataFile dataFile = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile),
                    new XmlRootAttribute { ElementName = "datafile" });
                serializer.UnknownElement += Serializer_UnknownElement;
                serializer.UnknownAttribute += Serializer_UnknownAttribute;
                StreamReader reader = new StreamReader(path);
                dataFile = (DataFile)serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DataFile),
                        new XmlRootAttribute { ElementName = "gameList" });
                    serializer.UnknownElement += Serializer_UnknownElement;
                    serializer.UnknownAttribute += Serializer_UnknownAttribute;
                    StreamReader reader = new StreamReader(path);
                    dataFile = (DataFile)serializer.Deserialize(reader);
                    dataFile.type = Type.EmulationStation;
                    reader.Close();
                }
                catch
                {

                    Console.WriteLine("Could not parse file: " + path);
                    return null;
                }
            }

            if (dataFile.Games.Count <= 0)
            {
                Console.WriteLine("Could not parse gamelist: no games found");
                return null;
            }

            // is datafile a mame "game" or "machine" nodes
            if (dataFile.Games[0] is Machine)
            {
                dataFile.type = Type.MameMachine;
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

            // create ObservableCollection for datagrid
            dataFile.gamesCollection = new ObservableCollection<Game>(dataFile.Games);

            // EmulationStation seems to have no header
            if (dataFile.Header == null)
            {
                dataFile.Header = new Header();
                dataFile.Header.Name = "No Name";
                dataFile.Header.Description = "No Description";
            }

            Console.WriteLine("database loaded: type is " + dataFile.type.ToString() + ", games: " + dataFile.Games.Count);

            return dataFile;
        }

        public static bool Save(DataFile dataFile, ItemCollection collection, string path)
        {
            try
            {
                TextWriter writer = new StreamWriter(path);
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
                // save visible games
                dataFile.Games = new List<Game>(collection.OfType<Game>());
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

        private static void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //Console.WriteLine("unk element: " + e.Element.Name);
            Game game = (Game)e.ObjectBeingDeserialized;

            // handle EmulationStation game nodes
            if (e.Element.Name == "name")
            {
                game.Name = e.Element.InnerText;
            }
        }

        private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //Console.WriteLine("unk element: " + e.Element.Name);
        }

        private static string GetEmbeddedResource(string resourceName)
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
