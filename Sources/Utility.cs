using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

    public static string ReadAsset(string path)
    {
        Uri textFileUri = new("avares://RetroFilter/Assets/" + path);
        using StreamReader streamReader = new(AssetLoader.Open(textFileUri), Encoding.UTF8);
        return streamReader.ReadToEnd();
    }

    public static MemoryStream? GetZipStream(string zipFile, string filename)
    {
        using var zip = ZipFile.OpenRead(zipFile);
        var entry = zip.GetEntry(filename);
        if (entry == null) return null;

        var memoryStream = new MemoryStream();
        using var zipStream = entry.Open();
        var buffer = new byte[1024 * 1024];
        int bytesRead;
        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, bytesRead);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public static bool Unzip(string zipFile, string filename, string dst)
    {
        using var inputStream = GetZipStream(zipFile, filename);
        if (inputStream == null)
        {
            Console.WriteLine("Zip.Extract: could not find compatible file in zip ({0})", zipFile);
            return false;
        }

        using var outputStream = new FileStream(dst, FileMode.Create);
        if (!outputStream.CanWrite)
        {
            Console.WriteLine("Zip.Extract: could not write to {0}", dst);
            return false;
        }

        inputStream.CopyTo(outputStream);
        return true;
    }

    public static bool UnzipFromAsset(string asset, string destination)
    {
        Uri textFileUri = new("avares://RetroFilter/Assets/" + asset);
        using var assetStream = AssetLoader.Open(textFileUri);
        if (!assetStream.CanRead) return false;

        // https://github.com/AvaloniaUI/Avalonia/issues/13604
        assetStream.Seek(0, SeekOrigin.Begin);
        using var ms = new MemoryStream();
        assetStream.CopyTo(ms);
        // https://github.com/AvaloniaUI/Avalonia/issues/13604

        using var zipArchive = new ZipArchive(ms, ZipArchiveMode.Read);
        foreach (var entry in zipArchive.Entries)
        {
            // Process each entry in the zip file
            using var entryStream = entry.Open();
            using var outputStream = new FileStream(destination, FileMode.Create);
            if (!outputStream.CanWrite)
            {
                Console.WriteLine("Zip.Extract: could not write to {0}", destination);
                return false;
            }

            entryStream.CopyTo(outputStream);
        }

        return true;
    }

    public static string Substring(string input, string start, string end)
    {
        try
        {
            var indexStart = input.IndexOf(start, StringComparison.Ordinal) + start.Length;
            var indexEnd = input.IndexOf(end, indexStart, StringComparison.Ordinal);
            return input.Substring(indexStart, indexEnd - indexStart).Trim();
        }
        catch (Exception e)
        {
            Console.WriteLine("Substring: " + e.Message);
        }

        return string.Empty;
    }

    public static bool IsDigits(string? str)
    {
        return str != null && str.All(c => c is >= '0' and <= '9');
    }
}