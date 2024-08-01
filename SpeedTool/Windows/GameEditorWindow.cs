using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Splits;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows;

public sealed class GameEditorWindow : Platform.Window
{
    private struct EditableCategory
    {
        public string Name;
        public Split[] Splits;
    }

    private readonly Platform.Platform platform;
    public GameEditorWindow() : base(WindowOptions.Default, new Vector2D<int>(800, 600))
    {
        platform = Platform.Platform.SharedPlatform;
        Name = "Game Name";
        categories = [];
        Text = "SpeedTool -- Game Editor";
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        TabCount = 0;

        ImGui.InputText("##name", ref Name, 255);
        ImGui.SameLine();
        ImGui.Text("Game Name");
        ImGui.Separator();
        if(ImGui.Button("Add category"))
        {
            var cat = new EditableCategory()
            {
                Name = GetNewCategoryName(),
                Splits = [new Split("New split")]
            };
            categories = categories.Append(cat).ToArray();
        }

        if(ImGui.BeginTabBar("Categories"))
        {
            for(int i = 0; i < categories.Length; i++)
                DoTab(ref categories[i].Name, ref categories[i].Splits);

            ImGui.EndTabBar();
        }

        ImGui.Separator();

        // Temporary stuff to debug this mess. Probably will be removed later
#if DEBUG
        DoDebugUI();
#endif

        ImGui.End();
    }

    private void DoTab(ref string name, ref Split[] splits)
    {
        if(ImGui.BeginTabItem(name + "###tabno" + TabCount.ToString()))
        {
            Split? editable = null;
            Split? parent = null;

            ImGui.InputText("Category Name", ref name, 256);
            ImGui.Text("Splits:");

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
                DoPopup(ref splits);
            
            if(!ImGui.IsPopupOpen(POPUP_NAME))
                popupSplit = null;

            ImGui.EndTabItem();
        }
        TabCount++;
    }

    private void DoPopup(ref Split[] splits)
    {
        if(ImGui.BeginPopupContextWindow(POPUP_NAME))
        {
            if(ImGui.MenuItem("Insert above"))
            {
                InsertAbove(ref splits);
                popupSplit = null;
            }
            if(ImGui.MenuItem("Insert below"))
            {
                InsertBelow(ref splits);
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

    private void InsertAbove(ref Split[] splits)
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

    private void InsertBelow(ref Split[] splits)
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

    private string GetNewCategoryName()
    {
        string name = "New Category";
        int tries = 0;
        while(categories.Any(x => x.Name == name))
        {
            tries++;
            name = "New Category " + tries.ToString();
        }
        return name;
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

    EditableCategory[] categories;

    private string Name = "";

    private int TabCount = 0;

    private const string POPUP_NAME = "popup_meun";
}
