using System.Diagnostics;
using System.Runtime.Versioning;

namespace SpeedTool.Platform.Linux;

[SupportedOSPlatform("Linux")]
public sealed class Fonts
{

    private static readonly IEnumerable<string> DEFAULT_FONTS = ["DroidSans.ttf", "NotoSans-Regular.ttf"];
    private static readonly IEnumerable<string> DEFAULT_CJK_FONTS = ["DroidSansJapanese.ttf", "NotoSansCJK-Regular.ttc"];
    public static string DefaultFont
    {
        get
        {
            if(!loaded)
                Load();

            var str = fonts.Where(x => DEFAULT_FONTS.Any(def_font => x.Contains(def_font))).FirstOrDefault();
            return str ?? "";
        }
    }

    public static string DefaultCJKFont
    {
        get
        {
            if(!loaded)
                Load();
            
            var str = fonts.Where(x => DEFAULT_CJK_FONTS.Any(def_font => x.Contains(def_font))).FirstOrDefault();
            return str ?? "";
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