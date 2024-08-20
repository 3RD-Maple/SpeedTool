namespace SpeedTool.Util;

public static class ENV
{
    public static string LocalFilesPath
    {
        get
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/speedtool/";
        }
    }
}
