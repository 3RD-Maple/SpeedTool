using SpeedTool.Global;
using SpeedTool.Global.Definitions;

namespace SpeedTool.Platform;

sealed class HotkeyController
{
    private sealed class HotkeyCycler
    {
        public HotkeyCycler(Hotkey hotkey, Action act)
        {
            hk = hotkey;
            action = act;
        }

        public void Cycle()
        {
            if(wasTriggered)
            {
                wasTriggered = hk.IsTriggered;
                return;
            }
            if(hk.IsTriggered)
            {
                wasTriggered = true;
                action();
            }
        }

        Action action;

        Hotkey hk;
        bool wasTriggered = false;
    }

    public HotkeyController()
    {
        RefreshSettings();
    }

    public void Cycle()
    {
        if(!enabled)
            return;

        foreach(var cyc in cyclers)
            cyc.Cycle();
    }

    public void RefreshSettings()
    {
        var hotkeys = Configuration.GetSection<HotkeySettings>()!;
        enabled = hotkeys.HotkeysEnabled;
        cyclers = 
        [
            new HotkeyCycler(hotkeys.SplitHotkey, () => Platform.SharedPlatform.Split()),
            new HotkeyCycler(hotkeys.NextSplitHotkey, () => Platform.SharedPlatform.NextSplit())
        ];
    }
    bool enabled = true;
    HotkeyCycler[] cyclers = [];
}
