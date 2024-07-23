using SpeedTool.Global;
using SpeedTool.Platform;
using SpeedTool.Windows;

var exeDir = Path.GetDirectoryName(System.AppContext.BaseDirectory);
Console.WriteLine(exeDir);
var configPath = Path.Combine(exeDir!, @"./App/appsettings.json");

Configuration.Init(configPath);
Platform.SharedPlatform.AddWindow(new MainWindow());

Platform.SharedPlatform.Run();
