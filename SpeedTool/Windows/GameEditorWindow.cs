using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Splits;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows;

public sealed class GameEditorWindow : Platform.Window
{
    private readonly Platform.Platform platform;
    public GameEditorWindow() : base(WindowOptions.Default, new Vector2D<int>(800, 600))
    {
        platform = Platform.Platform.SharedPlatform;
        splits = new Split[10];

        // This is temporary stuff for debug purposes :)
        for(int i = 0; i < 10; i++)
        {
            splits[i] = new Split();
            splits[i].Name = i.ToString();
            if(i == 4)
            {
                splits[i].Subsplits = new Split[4];
                for(int j = 0; j < 4; j++)
                {
                    splits[i].Subsplits[j] = new Split("Subsplit " + j.ToString());
                }
            }
        }
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ImGui.InputText("##name", ref Name, 255);
        ImGui.SameLine();
        ImGui.Text("Game Name");

        Split? editable = null;
        Split? parent = null;

        for(int i = 0; i < splits.Length; i++)
        {
            var res = ImGuiExtensions.SpeedToolSplit("##" + i.ToString(), ref splits[i]);
            if(res.selectedSplit != null)
                editable = res.selectedSplit;
            if(res.parentSplit != null)
                parent = res.parentSplit;

            if(popupSplit == null && editable != null)
            {
                popupSplit = editable;
                popupSplitParent = parent;
            }
        }

        if(popupSplit != null)
            DoPopup();

        if(!ImGui.IsPopupOpen(POPUP_NAME))
            popupSplit = null;

        // Temporary stuff to debug this mess. Probably will be removed later
#if DEBUG
        DoDebugUI();
#endif

        ImGui.End();
    }

    private void DoPopup()
    {
        if(ImGui.BeginPopupContextWindow(POPUP_NAME))
        {
            if(ImGui.MenuItem("Insert above"))
            {
                InsertAbove();
                popupSplit = null;
            }
            if(ImGui.MenuItem("Insert below"))
            {
                InsertBelow();
                popupSplit = null;
            }
            if(ImGui.MenuItem("Add subsplit"))
            {
                popupSplit!.AddSubsplit(new Split("New Split"));
                popupSplit = null;
            }
            ImGui.EndPopup();
        }
    }

    private void InsertAbove()
    {
        if(popupSplitParent != null)
        {
            popupSplitParent.InsertSplit(Array.IndexOf(popupSplitParent.Subsplits, popupSplit), new Split("New Split"));
        }
        else
        {
            var idx = Array.IndexOf(splits, popupSplit);
            splits = splits.InsertAt(idx, new Split("New Split"));
        }
    }

    private void InsertBelow()
    {
        if(popupSplitParent != null)
        {
            popupSplitParent.InsertSplit(Array.IndexOf(popupSplitParent.Subsplits, popupSplit) + 1, new Split("New Split"));
        }
        else
        {
            var idx = Array.IndexOf(splits, popupSplit) + 1;
            splits = splits.InsertAt(idx, new Split("New Split"));
        }
    }

#if DEBUG
    private void DoDebugUI()
    {
        if(popupSplit != null)
        {
            ImGui.Text("Selected split = " + popupSplit!.Name);
            if(popupSplitParent != null)
            {
                ImGui.Text("Parent split = " + popupSplitParent!.Name);
            }
        }
    }
#endif

    Split? popupSplit;
    Split? popupSplitParent;

    private Split[] splits;

    private string Name = "";

    private const string POPUP_NAME = "popup_meun";
}
