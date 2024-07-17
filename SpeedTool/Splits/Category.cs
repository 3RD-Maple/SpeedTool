namespace SpeedTool.Splits;

class Category
{
    public Category(string name)
    {
        Name = name;
        Splits = new Split[0];
    }

    public string Name { get; private set; }

    public Split[] Splits { get; private set; }

    public int RunsCount { get; private set;}
}
