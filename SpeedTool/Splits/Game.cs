using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Game
{
    public Game(string name, Category[] cats)
    {
        Name = name;
        categories = cats;
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

    public static Game LoadFromFile(string path)
    {
        Game g = new Game("", []);
        using(var file = ZipFile.OpenRead(path))
        {
            EnforceFileExists(file, "meta.json");
            var text = file.GetEntry("meta.json")!.AsText();
            var obj = JSONHelper.EnforceParseAsObject(text);
            g.Name = obj.EnforceGetString("Name");

            var categories = file.Entries.Where(x => x.FullName.StartsWith("category/"));
            foreach(var cat in categories)
            {
                Category.FromJson(JSONHelper.EnforceParseAsObject(cat.AsText()));
            }
        }
        return g;
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
        o[ExeName] = ExeName;

        return o;
    }

    private static void EnforceFileExists(ZipArchive archive, string file)
    {
        if(archive.GetEntry(file) == null)
            throw new FileLoadException($"Required file {file} missing in game archive");
    }
}
