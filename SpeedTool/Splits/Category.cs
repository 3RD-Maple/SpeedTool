namespace SpeedTool.Splits;

public class Category
{
    public Category(string name, Split[] splits)
    {
        Name = name;
        Splits = splits;
    }

    public string Name { get; set; }

    public Split[] Splits { get; set; }

    public int RunsCount { get; set; }
}
