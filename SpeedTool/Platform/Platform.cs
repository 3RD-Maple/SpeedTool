using ImGuiNET;

namespace SpeedTool.Platform;

public class Platform
{
    public static Platform SharedPlatform
    {
        get
        {
            if(platform is null)
            {
                platform = new Platform();
            }

            return platform!;
        }
    }

    public void LoadFont(string font, int size)
    {
        fonts.Add(ImGui.GetIO().Fonts.AddFontFromFileTTF(font, size));
    }

    public ImFontPtr GetFont(int idx)
    {
        return fonts[idx];
    }

    public void Run()
    {
        while (windows.Count != 0)
        {
            for(int i = 0; i < windows.Count; i++)
            {
                windows[i].Cycle();
            }

            var toClose = windows.Where(x => x.IsClosed).ToList();
            windows = windows.Where(x => !x.IsClosed).ToList();
            foreach (var closed in toClose)
            {
                closed.Reset();

                // FEXME: See Platform.Window class for explanation
                //closed.Dispose();
            }
        }
    }

    public void AddWindow(Window w)
    {
        windows.Add(w);
    }

    private Platform()
    {
        windows = new List<Window>();
        fonts = new List<ImFontPtr>();
    }

    List<Window> windows;
    List<ImFontPtr> fonts;

    private static Platform? platform;
}
