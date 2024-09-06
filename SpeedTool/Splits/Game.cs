using System.IO.Compression;
using System.Text.Json.Nodes;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Game
{
    public Game(string name, Category[] cats)
    {
        Name = name;
        categories = cats;
    }

    public Game(string name, string exeName, Category[] cats) : this(name, cats)
    {
        ExeName = exeName;
    }

    public Game(string name, string exeName, TimingMethod defTiming, string script, Category[] cats) : this(name, exeName, cats)
    {
        DefaultTimingMethod = defTiming;
        Script = script;
    }

    public bool HasCategory(string name)
    {
        return categories.Any(c => c.Name == name);
    }

    public Category GetCategoryByName(string category)
    {
        return categories.Where(c => c.Name == category).FirstOrDefault() ?? throw new KeyNotFoundException();
    }

    public void SaveToFile(string path)
    {
        using(var ms = new MemoryStream())
        {
            using(var file = new ZipArchive(ms, ZipArchiveMode.Create))
            {
                file.CreateEntry("meta.json", GetMetaJson().ToString());
                if(Script != "")
                {
                    file.CreateEntry("script.lua", Script);
                }

                for(int i = 0; i < categories.Length; i++)
                {
                    file.CreateEntry($"categories/{categories[i].Name}.json", GetCategoryJson(i).ToString());
                }
            }

            File.WriteAllBytes(path, ms.ToArray());
        }
    }

    /// <summary>
    /// Game's name
    /// </summary>
    public string Name { get; private set; } = "";

    /// <summary>
    /// Name of the game's executable file
    /// </summary>
    public string ExeName { get; private set; } = "";

    /// <summary>
    /// Preffered timing for this game. Default is `TimingMethod.RealTime`
    /// </summary>
    public TimingMethod DefaultTimingMethod { get; private set; } = TimingMethod.RealTime;

    /// <summary>
    /// Game script to run on the injected timer
    /// </summary>
    public string Script { get; private set; } = "";

    public static Game LoadFromFile(string path)
    {
        Game g = new Game("", []);
        g.source = ZipFile.OpenRead(path);

        EnforceFileExists(g.source, "meta.json");
        var text = g.source.GetEntry("meta.json")!.AsText();
        var obj = JSONHelper.EnforceParseAsObject(text);
        g.Name = obj.EnforceGetString("Name");
        if(obj.ContainsKey("DefaultTimingMethod"))
            g.DefaultTimingMethod = (TimingMethod)obj.EnforceGetInt("DefaultTimingMethod");
        if(obj.ContainsKey("ExeName"))
            g.ExeName = obj.EnforceGetString("ExeName");

        var script = g.source.GetEntry("script.lua");
        if(script != null)
        {
            g.Script = script!.AsText();
        }

        var categories = g.source.Entries.Where(x => x.FullName.StartsWith("categories/")).ToArray();
        g.categories = new Category[categories.Length];
        for(int i = 0; i < categories.Length; i++)
        {
            g.categories[i] = Category.FromJson(JSONHelper.EnforceParseAsObject(categories[i].AsText()));
        }

        g.source.Dispose();
        g.source = null;

        return g;
    }

    public Category[] GetCategories()
    {
        return categories;
    }

    private Category[] categories;

    private JsonObject GetCategoryJson(int idx)
    {
        return categories[idx].ToJson();
    }

    private JsonObject GetMetaJson()
    {
        JsonObject o = new JsonObject();
        o["Name"] = Name;
        o["DefaultTimingMethod"] = (int)DefaultTimingMethod;
        o["ExeName"] = ExeName;

        return o;
    }

    private static void EnforceFileExists(ZipArchive archive, string file)
    {
        if(archive.GetEntry(file) == null)
            throw new FileLoadException($"Required file {file} missing in game archive");
    }
    private ZipArchive? source;
}
