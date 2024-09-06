using System.Runtime.InteropServices;
using SpeedTool.Platform.Linux;
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
            SaveFileLinuxOrMacOS(onSave);
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
            OpenFileLinuxOrMacOS(onLoad);
        }
    }
    
    public static void OpenFileLinuxOrMacOS(Action<string> onLoad)
    {
        var dialog = new UniversalFileDialog(onLoad, DialogOperation.Open);
        Platform.Platform.SharedPlatform.AddWindow(dialog);
        dialog.OpenFolder(onLoad);
    }
    
    public static void SaveFileLinuxOrMacOS(Action<string> onSave)
    {
        var dialog = new UniversalFileDialog(onSave, DialogOperation.Save);
        Platform.Platform.SharedPlatform.AddWindow(dialog);
    }
}
