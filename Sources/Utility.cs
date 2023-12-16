using System;
using System.IO;
using System.Text;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Platform.Storage;

namespace RetroFilter.Sources;

public abstract class Utility
{
    public static FilePickerFileType DatabaseFilter { get; } = new("Mame / EmulationStation (*.dat/*.xml)")
    {
        Patterns = new[] { "*.dat", "*.xml" },
        MimeTypes = new[] { "dat/*" }
    };

    public static string ReadAsset(string path)
    {
        Uri textFileUri = new("avares://RetroFilter/Assets/" + path);
        using StreamReader streamReader = new(AssetLoader.Open(textFileUri), Encoding.UTF8);
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