using SpeedTool.Global;
using SpeedTool.Global.Definitions;

namespace SpeedTool.Platform;

sealed class HotkeyController
{
    private sealed class HotkeyCycler
    {
        public HotkeyCycler(Hotkey hotkey, Action act, bool ignoreGlobal = false)
        {
            hk = hotkey;
            action = act;
            IgnoreGlobalSettings = ignoreGlobal;
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

        public bool IgnoreGlobalSettings { get; private set; } = false;
    }

    public HotkeyController()
    {
        RefreshSettings();
    }

    public void Cycle()
    {
        if(!enabled)
        {
            foreach(var cyc in cyclers.Where(x => x.IgnoreGlobalSettings))
                cyc.Cycle();
            return;
        }

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
            new HotkeyCycler(hotkeys.NextSplitHotkey, () => Platform.SharedPlatform.NextSplit()),
            new HotkeyCycler(hotkeys.PreviousSplitHotkey, () => Platform.SharedPlatform.PreviousSplit()),
            new HotkeyCycler(hotkeys.NextCategoryHotkey, () => Platform.SharedPlatform.NextCategory()),
            new HotkeyCycler(hotkeys.PreviousCategoryHotkey, () => Platform.SharedPlatform.PreviousCategory()),
            new HotkeyCycler(hotkeys.ToggleHotkeysHotkey, () => ToggleHotkeys(), true)
        ];
    }

    private void ToggleHotkeys()
    {
        var settings = Configuration.GetSection<HotkeySettings>()!;
        settings.HotkeysEnabled = !settings.HotkeysEnabled;
        Configuration.SetSection(settings);
        RefreshSettings();
    }

    bool enabled = true;
    HotkeyCycler[] cyclers = [];
}
