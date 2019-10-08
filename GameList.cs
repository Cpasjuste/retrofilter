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

        public enum Format
        {
            Mame
        }

        public Format format = Format.Mame;
        public string name;
        public string description;
        public string category;
        public string version;
        public string author;
        public string homepage;
        public string url;

        public ObservableCollection<Game> games { get; set; }

        [Serializable()]
        [XmlRoot("datafile", Namespace = "", IsNullable = false)]
        public class DataFile
        {
            [XmlElement(ElementName = "game")]
            public List<Game> Game { get; set; }
        }

        public bool Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataFile));
            StreamReader reader = new StreamReader(path);
            var dataFile = (DataFile)serializer.Deserialize(reader);
            games = new ObservableCollection<Game>(dataFile.Game);
            reader.Close();

            /*
            games = new List<Game>();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            cadtch
            {
                Console.WriteLine("Could not open gamelist: " + path);ijuu  *ùqâvvvv                return false;
            }

            // try to detect gamelist format (mame dat only for now)
            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("/datafile/header");
                format = Format.Mame;
                name = getNodeText(node, "name");
                description = getNodeText(node, "description");
                category = getNodeText(node, "category");
                version = getNodeText(node, "version");
                author = getNodeText(node, "author");
                homepage = getNodeText(node, "homepage");
                url = getNodeText(node, "url");

                Console.WriteLine();
                Console.WriteLine("GameList: " + name);
                Console.WriteLine("\tdescription: " + description);
                Console.WriteLine("\tcategory: " + category);
                Console.WriteLine("\tversion: " + version);
                Console.WriteLine("\tauthor: " + author);
                Console.WriteLine("\thomepage: " + homepage);
                Console.WriteLine("\turl: " + url);
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine("Could not process gamelist: \'/datafile/header\' tag not found");
                return false;
            }

            try
            {
                XmlNodeList gameList = doc.SelectNodes("/datafile/game");
                foreach (XmlNode node in gameList)
                {
                    Game game = new Game();
                    game.Name = getAttributeText(node, "name");
                    game.CloneOf = getAttributeText(node, "cloneof");
                    game.IsClone = !string.IsNullOrEmpty(game.CloneOf);
                    game.RomOf = getAttributeText(node, "romof");
                    game.Description = getNodeText(node, "description");
                    game.Year = getNodeText(node, "year");
                    game.Manufacturer = getNodeText(node, "manufacturer");
                    game.Status = getNodeText(node, "status");
                    games.Add(game);
                }

                Console.WriteLine("Games in gamelist: " + games.Count);
            }
            catch
            {
                Console.WriteLine("Could not process gamelist: \'/datafile/game\' tags not found");
                return false;
            }
            */

            return true;
        }

        public bool Save(string path)
        {


            return true;
        }

        private string getNodeText(XmlNode node, string childName)
        {
            string text = null;

            if (node != null && node.SelectSingleNode(childName) != null)
            {
                text = node.SelectSingleNode(childName).InnerText;
            }

            return text;
        }

        private string getAttributeText(XmlNode node, string name)
        {
            string text = null;

            if (node != null && node.Attributes[name] != null)
            {
                text = node.Attributes[name].InnerText;
            }

            return text;
        }

    }
}
