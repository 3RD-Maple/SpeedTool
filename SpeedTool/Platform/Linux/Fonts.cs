using System.Diagnostics;
using System.Runtime.Versioning;
using SpeedTool.Platform.Debugging;

namespace SpeedTool.Platform.Linux;

[SupportedOSPlatform("Linux")]
public sealed class Fonts
{
    public static string DefaultFont
    {
        get
        {
            if(!loaded)
                Load();

            return defaultFont ?? "";
        }
    }

    public static string DefaultCJKFont
    {
        get
        {
            if(!loaded)
                Load();

            return defaultCJKFont ?? "";
        }
    }

    private static void Load()
    {
        loaded = true;

        defaultFont = FontconfigMatch("sans-serif") ?? FindByNames(StandardFontNames) ?? "";
        defaultCJKFont = FontconfigMatch("sans-serif:lang=ja") ?? FindByNames(CJKFontNames) ?? "";

        if(defaultFont == "")
            DebugLog.SharedInstance.Write("Fonts: no usable system font found, falling back to the built-in ImGui font");
    }

    private static string? FontconfigMatch(string pattern)
    {
        try
        {
            using(var proc = new Process())
            {
                proc.StartInfo = new()
                {
                    FileName = "fc-match",
                    Arguments = $"-f \"%{{file}}\" {pattern}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                if(!proc.Start())
                    return null;

                var file = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit();

                return File.Exists(file) ? file : null;
            }
        }
        catch
        {
            return null;
        }
    }

    private static string? FindByNames(string[] names)
    {
        var files = ListFontFiles();
        foreach(var name in names)
        {
            var str = files.Where(x => x.ToLowerInvariant().Contains(name)).FirstOrDefault();
            if(str != null)
                return str;
        }

        return null;
    }

    private static string[] ListFontFiles()
    {
        var files = new List<string>();
        files.AddRange(RunFcList());

        // Scan standard font directories as well, fc-list might be missing or not in PATH
        foreach(var dir in FontDirs)
        {
            if(!Directory.Exists(dir))
                continue;

            files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                                     .Where(x => x.EndsWith(".ttf") || x.EndsWith(".otf")));
        }

        return files.Distinct().ToArray();
    }

    private static IEnumerable<string> RunFcList()
    {
        try
        {
            using(var proc = new Process())
            {
                proc.StartInfo = new()
                {
                    FileName = "fc-list",
                    Arguments = ": file",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                if(!proc.Start())
                    return [];

                var files = proc.StandardOutput.ReadToEnd()
                                             .Split("\n")
                                             .Select(x => x.Trim())
                                             .Where(x => x.EndsWith(":"))
                                             .Select(x => x.Substring(0, x.Length - 1).Trim())
                                             .Where(x => x.EndsWith(".ttf") || x.EndsWith(".otf"))
                                             .ToArray();

                proc.WaitForExit();
                return files;
            }
        }
        catch
        {
            return [];
        }
    }

    private static readonly string[] StandardFontNames =
    [
        "dejavusans", "notosans", "liberationsans", "droidsans", "opensans", "firasans", "freesans", "ubuntu"
    ];

    private static readonly string[] CJKFontNames =
    [
        "notosanscjk", "droidsansfallback", "droidsansjapanese", "sourcehans", "wenquanyi", "vlgothic", "takao", "ipafont", "nanum"
    ];

    private static readonly string[] FontDirs =
    [
        "/usr/share/fonts",
        "/usr/local/share/fonts",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.local/share/fonts",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.fonts"
    ];

    private static string? defaultFont;
    private static string? defaultCJKFont;
    private static bool loaded = false;
}
