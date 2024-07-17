using ImGuiNET;

namespace SpeedTool.Windows.Settings;

public abstract class TabBase
{
    public void DoTab(string tabBarName)
    {
        if (ImGui.BeginTabBar(tabBarName))
        {
            DoTabInternal();
            ImGui.EndTabBar();
        }
    }
    protected abstract void DoTabInternal();
}