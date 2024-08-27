using ImGuiNET;
using SpeedTool.Global;

namespace SpeedTool.Windows.Settings;

public abstract class TabBase : IDisposable
{
    public string TabName { get; private set; }

    protected TabBase(string tabName)
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
    
    private void HandleConfigChanges(object? sender, IConfigurationSection section)
    {
        OnConfigChanges(sender, section);
    }

    protected virtual void OnConfigChanges(object? sender, IConfigurationSection section)
    {
    }

    public virtual void Dispose()
    {            
        Configuration.OnConfigurationChanged -= HandleConfigChanges;
    }
}