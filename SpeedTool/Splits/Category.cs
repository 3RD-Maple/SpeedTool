namespace SpeedTool.Splits;

public class Category
{
    public Category(string name, Split[] splits)
    {
        Name = name;
        Splits = splits;
    }

    public string Name { get; private set; }

    public Split[] Splits { get; private set; }

    public int RunsCount { get; private set;}
}
