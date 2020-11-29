using System;
using System.Collections.Generic;
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
            Mame2003,
            EmulationStation
        };

        [XmlElement(ElementName = "header")]
        public Header Header { get; set; }
        [XmlElement("game", typeof(Game))]
        [XmlElement("machine", typeof(Machine))]
        public List<Game> Games { get; set; }
        [XmlIgnore]
        public Type type = Type.MameGame;

        public List<string> GetFilteredColumns()
        {
            List<string> filters = new List<string>();

            // hide non handled items
            filters.AddRange(new List<string>() {
                "isbios_","isdevice_","runnable_","driver" ,"biosset",
                "rom" ,"device_ref","sample", "disk", "chip", //"video", <- now used es/recalbox
                "sound", "input", "dipswitch"
            });

            if (type != Type.EmulationStation)
            {
                filters.AddRange(new List<string>()
                {  "nameES", "desc", "source", "image", "thumbnail", "releasedate",
                    "path", "developer", "region", "romtype", "id",
                    "publisher", "rating", "players", "hash"});
            }

            if (type == Type.MameGame)
            {
                filters.AddRange(new List<string>()
                {  "sourcefile", "isdevice", "runnable" });
            }
            else if (type == Type.Mame2003)
            {
                filters.AddRange(new List<string>()
                {  "sourcefile" });
            }
            else if (type == Type.EmulationStation)
            {
                filters.AddRange(new List<string>()
                {  "name", "sourcefile", "isdevice", "runnable",
                    "isbios", "year", "description","manufacturer",
                    "romof", "cloneof", "isrom", "isclone" });
            }

            return filters;
        }

        public static DataFile Load(string path)
        {
            // mame.dat
            DataFile dataFile = Load(path, "datafile");
            if (dataFile == null)
            {
                // mame2003-plus.xml
                Console.WriteLine("trying mame2003 format...");
                if ((dataFile = Load(path, "mame")) != null)
                {
                    dataFile.type = Type.Mame2003;
                }
            }

            if (dataFile == null)
            {
                // gamelist.xml
                Console.WriteLine("trying gamelist.xml format...");
                if ((dataFile = Load(path, "gameList")) != null)
                {
                    dataFile.type = Type.EmulationStation;
                }
            }

            if (dataFile == null)
            {
                Console.WriteLine("Could not parse file: " + path);
                return null;
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
                string name = string.IsNullOrEmpty(game.name) ? game.nameES : game.name;
                if (!catverDic.TryGetValue(name, out string genre) && game.isclone)
                {
                    catverDic.TryGetValue(game.cloneof, out genre);
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    game.genre = genre;
                }
            }

            // EmulationStation seems to have no header
            if (dataFile.Header == null)
            {
                dataFile.Header = new Header
                {
                    Name = "No Name",
                    Description = "No Description"
                };
            }

            Console.WriteLine("database loaded: type is " + dataFile.type.ToString() + ", games: " + dataFile.Games.Count);

            return dataFile;
        }

        public static bool Save(DataFile dataFile, ItemCollection collection, string path)
        {
            try
            {
                string root = dataFile.type == Type.EmulationStation ? "gameList" : "datafile";
                if (dataFile.type == Type.Mame2003)
                {
                    root = "mame";
                }
                XmlRootAttribute xmlRoot = new XmlRootAttribute(root);
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile), xmlRoot);
                DataFile df = new DataFile();
                df.Header = dataFile.type == Type.EmulationStation ? null : dataFile.Header;
                // only save visible games
                df.Games = new List<Game>(collection.OfType<Game>());
                TextWriter writer = new StreamWriter(path);
                serializer.Serialize(writer, df);
                writer.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse gamelist: " + path);
                return false;
            }
            return true;
        }

        private static DataFile Load(string path, string root)
        {
            DataFile dataFile = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataFile),
                    new XmlRootAttribute { ElementName = root });
                StreamReader reader = new StreamReader(path);
                dataFile = (DataFile)serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
                Console.WriteLine("Could not parse file: " + path);
            }

            return dataFile;
        }

        private static void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //Console.WriteLine("unk element: " + e.Element.Name);
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
