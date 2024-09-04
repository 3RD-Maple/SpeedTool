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
            throw new NotImplementedException();
    }

    public static void OpenFile(Action<string> onLoad)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Dialogs.ShowOpenDialog(onLoad);
        }
        else
            throw new NotImplementedException();
    }
    
    public static void OpenFileLinuxOrMacOS()
    {
        Platform.Platform.SharedPlatform.AddWindow(new UniversalFileDialog());
    }
}
