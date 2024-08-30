using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SpeedTool.Platform.Windows;

[SupportedOSPlatform("Windows")]
public static class WindowsFileDialog
{
    public static void ShowSaveDialog(Action<string> onSave)
    {
        OPENFILENAME openFileName = new();
        openFileName.lpstrDefExt = "stg";
        openFileName.nMaxFile = 255;
        openFileName.Flags = (int)OpenFileNameFlags.OFN_OVERWRITEPROMPT;
        openFileName.lpstrFilter = "SpeedTool Game files (*.stg)\0*.stg";
        if(GetSaveFileName(openFileName))
        {
            onSave(openFileName.lpstrFile);
        }
    }

    public static void ShowOpenDialog(Action<string> onOpen)
    {
        OPENFILENAME openFileName = new();
        openFileName.lpstrDefExt = "lua";
        openFileName.nMaxFile = 255;
        openFileName.Flags = (int)OpenFileNameFlags.OFN_OVERWRITEPROMPT;
        openFileName.lpstrFilter = "Lua scripts (*.lua)\0*.lua";
        if(GetOpenFileName(openFileName))
        {
            onOpen(openFileName.lpstrFile);
        }
    }

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OPENFILENAME unnamedParam1);

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OPENFILENAME unnamedParam1);
}
