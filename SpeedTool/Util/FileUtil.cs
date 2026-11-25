using System.Runtime.InteropServices;
using Silk.NET.SDL;
using SpeedTool.Platform.Linux;
using SpeedTool.Windows;
using Dialogs = SpeedTool.Platform.Windows.WindowsFileDialog;


namespace SpeedTool.Util;

public static class FileUtil
{
    public static void SaveFile(Action<string> onSave)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Dialogs.ShowSaveDialog(onSave);
        }
        else
        {
            var font = ImGuiNET.ImGui.GetFont();
            ImGuiNET.ImGui.PopFont();
            
            SaveFileLinuxOrMacOS(onSave);
            
            ImGuiNET.ImGui.PushFont(font);
        }
    }

    public static void OpenFile(Action<string> onLoad)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        { 
            Dialogs.ShowOpenDialog(onLoad);
        }
        else
        {
            var font = ImGuiNET.ImGui.GetFont();
            ImGuiNET.ImGui.PopFont();
            
            OpenFileLinuxOrMacOS(file =>
            {
                if (File.Exists(file))
                { 
                    File.ReadAllText(file);
                }
            });
            
            ImGuiNET.ImGui.PushFont(font);
        }
    }
    
    private static void OpenFileLinuxOrMacOS(Action<string> onLoad)
    {
        var dialog = new UniversalFileDialog(onLoad, DialogOperation.Open);
        Platform.Platform.SharedPlatform.AddWindow(dialog);
    }
    
    private static void SaveFileLinuxOrMacOS(Action<string> onSave)
    {
        var dialog = new UniversalFileDialog(onSave, DialogOperation.Save);
        Platform.Platform.SharedPlatform.AddWindow(dialog);
    }
}
