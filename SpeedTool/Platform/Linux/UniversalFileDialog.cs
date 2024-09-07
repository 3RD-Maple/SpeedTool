using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SpeedTool.Platform.Linux;

public sealed class UniversalFileDialog : Window
{
    private UniversalDirectory directory;

    public UniversalFileDialog(Action<string> onLoad, DialogOperation operation) : base(options, new Vector2D<int>(500, 550))
    {
        directory = new UniversalDirectory(AppContext.BaseDirectory);
        this.onLoad = onLoad;
        operationMode = operation;
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
                    isfolderSelected = directory.Directories!.Contains(directory.TotalFiles[0]);
                }
            }
        }

       

        ImGui.Text(directory.CurrentPath);

        if (ImGui.BeginListBox("", new Vector2(ImGui.GetWindowWidth() * 1f, ImGui.GetWindowHeight() * 0.6f)))
        {
            for (int i = 0; i < directory.TotalFiles!.Count; i++)
            {
                bool isSelected = (currentItem == i);
                if (ImGui.Selectable(directory.TotalFiles[i], isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentItem = i;
                    selectedDirectory = Path.Combine(directory.CurrentPath, directory.TotalFiles[i]);
                    isfolderSelected = directory.Directories!.Contains(directory.TotalFiles[i]);

                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        OpenFolder(onLoad);
                    }
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
        }
        ImGui.EndListBox();

        
        
        
        ImGui.InputText("", ref fileName, 255);
        
        ImGui.SameLine();
        
        
        ImGui.SameLine();
        
        if (operationMode == DialogOperation.Open)
        {
            if (ImGui.Button("Open", new Vector2(ImGui.GetWindowWidth() * 0.3f, ImGui.GetWindowHeight() * 0.05f)))
            {
                if (isfolderSelected)
                {
                    OpenFolder(path => onLoad(path));
                }
            }
        }
        else
        {
           
            if (ImGui.Button("Save", new Vector2(ImGui.GetWindowWidth() * 0.3f, ImGui.GetWindowHeight() * 0.05f)))
            {
                SaveFile(path => onLoad(path));
            }
        }
        
        if (ImGui.Button("Cancel", new Vector2(ImGui.GetWindowWidth() * 0.4f, ImGui.GetWindowHeight() * 0.05f)))
        {
            Close();
        }
    }

    
    
    public void OpenFolder(Action<string> onOpen)
    {
        if (Directory.Exists(selectedDirectory))
        {
            directory.CurrentPath = selectedDirectory;
            directory.RefreshDirectory();

            if (directory.TotalFiles!.Count > 0)
            {
                selectedDirectory = Path.Combine(directory.CurrentPath, directory.TotalFiles[0]);
                isfolderSelected = directory.Directories!.Contains(directory.TotalFiles[0]);

                onOpen(selectedDirectory);
            }
        }
        else
        {
            onOpen(Path.GetFullPath(directory.CurrentPath));
            onOpen?.Invoke(Path.GetFullPath(directory.CurrentPath));
            Close();
        }
    }
    
    public void SaveFile(Action<string> onSave)
    {
        var fullPath = Path.Combine(directory.CurrentPath, fileName) + ".stg";

       if (!string.IsNullOrEmpty(fileName))
       {
           onSave?.Invoke(fullPath);
       }
    }

    private readonly DialogOperation operationMode;

    private string fileName = "";
    
    private Action<string> onLoad;
    
    private string? selectedDirectory = AppContext.BaseDirectory;

    private bool isfolderSelected;

    private int currentItem;
}