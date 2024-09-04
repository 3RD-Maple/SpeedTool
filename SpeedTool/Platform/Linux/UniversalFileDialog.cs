using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SpeedTool.Platform.Linux;

public sealed class UniversalFileDialog : Window
{

    private UniversalDirectory directory;

    public UniversalFileDialog() : base(options, new Vector2D<int>(500, 550))
    {
        directory = new UniversalDirectory(AppContext.BaseDirectory.ToString());
    }

    private static WindowOptions options
    {
        get
        {
            var opts = WindowOptions.Default;
            opts.Samples = 8;
            return opts;
        }
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("LinuxDialog",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoMove);

        ImGui.PushFont(GetFont("UI"));

        directory.RefreshDirectory();
        
        DrawWindow();
        
        ImGui.PopFont();
        
        ImGui.End();
    }

    private void DrawWindow()
    {
        if (ImGui.Button("Back", new Vector2(ImGui.GetWindowWidth() * 0.1f, ImGui.GetWindowHeight() * 0.1f)))
        {
            var parentDirectory = (Directory.GetParent(directory.CurrentPath));
            if (!(parentDirectory is null))
            {
                directory.CurrentPath = parentDirectory.ToString();
                directory.RefreshDirectory(); 
                
                if (directory.TotalFiles!.Count > 0)
                {
                    selectedDirectory = Path.Combine(directory.CurrentPath, directory.TotalFiles[0]);
                    folderSelected = directory.Directories!.Contains(directory.TotalFiles[0]);
                }
            }
        }

        ImGui.SameLine();
        
        ImGui.BeginDisabled(!folderSelected);
        
        if (ImGui.Button("Open", new Vector2(ImGui.GetWindowWidth() * 0.1f, ImGui.GetWindowHeight() * 0.1f)))
        {
            if (folderSelected)
            {
                OpenFolder(selectedDirectory!);
            }
        }
        
        ImGui.EndDisabled();

        ImGui.Text(directory.CurrentPath);

        if (ImGui.BeginListBox("", new Vector2(ImGui.GetWindowWidth() * 0.75f, ImGui.GetWindowHeight() * 0.6f)))
        {
            for (int i = 0; i < directory.TotalFiles!.Count; i++)
            {
                bool isSelected = (currentItem == i);
                if (ImGui.Selectable(directory.TotalFiles[i], isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentItem = i;
                    selectedDirectory = Path.Combine(directory.CurrentPath, directory.TotalFiles[i]);
                    folderSelected = directory.Directories!.Contains(directory.TotalFiles[i]);

                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        OpenFolder(selectedDirectory);
                    }
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
        }
        ImGui.EndListBox();
    }

    
    private void OpenFolder(string selectedFolder)
    {
        if (Directory.Exists(selectedFolder))
        {
            directory.CurrentPath = selectedFolder;
            directory.RefreshDirectory();

            if (directory.TotalFiles!.Count > 0)
            {
                selectedDirectory = Path.Combine(directory.CurrentPath, directory.TotalFiles[0]);
                folderSelected = directory.Directories!.Contains(directory.TotalFiles[0]);
            }
        }
    }

    private string? selectedDirectory = AppContext.BaseDirectory;

    private bool folderSelected;

    private int currentItem;
}