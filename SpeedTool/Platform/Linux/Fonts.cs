using System.Diagnostics;
using System.Runtime.Versioning;

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

            var str = fonts.Where(x => x.Contains("DroidSans.ttf")).FirstOrDefault();
            return str == null ? "" : str;
        }
    }

    public static string DefaultCJKFont
    {
        get
        {
            if(!loaded)
                Load();
            
            var str = fonts.Where(x => x.Contains("DroidSansJapanese.ttf")).FirstOrDefault();
            return str == null ? "" : str;
        }
    }

    private static void Load()
    {
        using(var proc = new Process())
        {
            loaded = true;
            proc.StartInfo = new()
            {
                FileName = "fc-list",
                Arguments = ": file",
                RedirectStandardOutput = true
            };

            if(!proc.Start())
                return;

            fonts = proc.StandardOutput.ReadToEnd()
                                       .Split("\n")
                                       .Where(x => x.Length > 0)
                                       .Select(x => x.Substring(0, x.Length - 2))
                                       .Where(x => x.EndsWith(".ttf"))
                                       .ToArray();

            proc.WaitForExit();
        }
    }

    private static string[] fonts = [];
    private static bool loaded = false;
}