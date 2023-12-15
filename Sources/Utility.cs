using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Platform;

namespace RetroFilter.Sources;

public abstract class Utility
{
    public static string ReadAsset(string path)
    {
        Uri textFileUri = new("avares://RetroFilter/Assets/" + path);
        using StreamReader streamReader = new(AssetLoader.Open(textFileUri));
        return streamReader.ReadToEnd();
    }

    public static T? FindParent<T>(Control? control) where T : Control
    {
        if (control == null) return null;
        var parent = control.Parent as Control;
        while (parent != null)
        {
            if (parent is T typedParent) return typedParent;
            parent = parent.Parent as Control;
        }

        return null;
    }
}