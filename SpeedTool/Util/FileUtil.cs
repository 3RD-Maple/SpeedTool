using System.Runtime.InteropServices;
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
}