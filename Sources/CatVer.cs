using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetroFilter.Sources;

public class CatVer
{
    public string Name { get; set; } = "Unknown";

    public string Date { get; set; } = "Unknown";

    public Dictionary<string, string> Mapping = new();

    public CatVer()
    {
        if (!File.Exists("catver.ini")) Utility.UnzipFromAsset("catver.zip", "catver.ini");

        var text = File.ReadAllText("catver.ini");
        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("error: could not load catver.ini...");
            return;
        }

        // parse catver name
        var infoLine = Utility.Substring(text, ";;", ";;");
        var split = infoLine.Split("/");
        if (split.Length == 3)
        {
            Name = split[2].Trim();
            Date = split[1].Trim();
        }

        var catVerText = Utility.Substring(text, "[Category]", "[VerAdded]");
        var lines = catVerText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Mapping = lines.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
    }
}