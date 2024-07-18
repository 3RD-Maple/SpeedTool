using ImGuiNET;

namespace SpeedTool.Windows.Settings;

public abstract class TabBase
{
    private string? tabName;

    public TabBase(string tabName)
    {
        this.tabName = tabName;
    }
    

    public void DoTab()
    {
        if (ImGui.BeginTabItem(tabName))
        {
            DoTabInternal();
            ImGui.EndTabItem();
        }
    }
    protected abstract void DoTabInternal();
}