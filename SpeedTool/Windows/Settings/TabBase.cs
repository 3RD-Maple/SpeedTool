using ImGuiNET;

namespace SpeedTool.Windows.Settings;

public abstract class TabBase
{
    public string TabName { get; private set; }

    public TabBase(string tabName)
    {
        TabName = tabName;
    }
    
    
    public void DoTab()
    {
        if (ImGui.BeginTabItem(TabName))
        {
            DoTabInternal();
            ImGui.EndTabItem();
        }
    }

    public void ApplySettings()
    {
        ApplyTabSettings();
    }
    protected abstract void DoTabInternal();

    protected abstract void ApplyTabSettings();
}