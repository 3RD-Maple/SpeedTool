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
    protected abstract void DoTabInternal();
}