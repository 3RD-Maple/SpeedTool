namespace SpeedTool.Platform.Linux;

public sealed class UniversalDirectory
{

    public UniversalDirectory(string curDir)
    {
        CurrentPath = curDir;
        
        TotalFiles = Directories.Concat(Files).ToList();
    }
    
    public void RefreshDirectory()
    {
        Directories = Directory.GetDirectories(CurrentPath)
            .Select(x => Path.GetFileName(x))
            .ToArray();
        
        Files = Directory.GetFiles(CurrentPath)
            .Select(x => Path.GetFileName(x))
            .ToArray();
        
        TotalFiles = Directories
            .Concat(Files)
            .ToList();
    }
    
    public string CurrentPath{ get; set; }
    
    public IEnumerable<string>? Directories { get; set; } = Directory.GetDirectories(AppContext.BaseDirectory).Select(x => Path.GetFileName(x)).ToArray();
    
    public IEnumerable<string>? Files { get; set; } = Directory.GetFiles(AppContext.BaseDirectory).Select(x => Path.GetFileName(x)).ToArray();

    public List<string>? TotalFiles { get; set; } 
}