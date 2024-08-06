using System.IO.Compression;

namespace SpeedTool.Util;

public static class ZipArchiveExtension
{
    public static void CreateEntry(this ZipArchive archive, string name, string data, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        var entry = archive.CreateEntry(name, compressionLevel);
        using(var stream = entry.Open())
        using(var sr = new StreamWriter(stream))
        {
            sr.Write(data);
        }
    }

    public static string AsText(this ZipArchiveEntry entry)
    {
        using(var ms = new StreamReader(entry.Open()))
        {
            return ms.ReadToEnd();
        }
    }
}
