using SpeedTool.Platform;
using SpeedTool.Windows;

Platform.SharedPlatform.AddWindow(new MainWindow());
Platform.SharedPlatform.AddWindow(new SettingsWindow());
Platform.SharedPlatform.Run();