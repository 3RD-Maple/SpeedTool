using SpeedTool.Global;
using SpeedTool.Platform;
using SpeedTool.Windows;

Configuration.Init(@".\App\appsettings.json");
Platform.SharedPlatform.AddWindow(new MainWindow());
Platform.SharedPlatform.AddWindow(new SettingsWindow());

Platform.SharedPlatform.Run();