namespace SpeedTool.Splits;

public class Game
{
    public Game()
    {
        categories = new Category[0];
    }

    public bool HasCategory(string name)
    {
        return categories.Any(c => c.Name == name);
    }

    public Category GetCategoryByName(string category)
    {
        return categories.Where(c => c.Name == category).FirstOrDefault() ?? throw new KeyNotFoundException();
    }

    /// <summary>
    /// Game's name
    /// </summary>
    public string Name { get; private set; } = "";

    /// <summary>
    /// Name of the game's executable file
    /// </summary>
    public string ExeName { get; private set; } = "";

    private Category[] categories;
}
