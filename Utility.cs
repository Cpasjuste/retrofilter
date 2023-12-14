using System;
using System.IO;
using Avalonia.Platform;

namespace RetroFilter;

public abstract class Utility
{
    public static string ReadAsset(string path)
    {
        Uri textFileUri = new("avares://RetroFilter/Assets/" + path);
        using StreamReader streamReader = new(AssetLoader.Open(textFileUri));
        return streamReader.ReadToEnd();
    }
}