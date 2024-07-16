using System.Reflection;
using SpeedTool.Global;
using SpeedTool.Platform;
using SpeedTool.Windows;


var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
Console.WriteLine(exeDir);
var configPath = Path.Combine(exeDir!, @"./App/appsettings.json");

Configuration.Init(configPath);
Platform.SharedPlatform.AddWindow(new MainWindow());
Platform.SharedPlatform.AddWindow(new SettingsWindow());
Platform.SharedPlatform.AddWindow(new GameEditorWindow());

Platform.SharedPlatform.Run();
