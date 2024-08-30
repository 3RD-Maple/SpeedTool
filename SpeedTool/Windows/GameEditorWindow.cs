using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Splits;
using SpeedTool.Timer;
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
        categories = [new EditableCategory(){
            Name = "any%",
            Splits = [new Split("New Split")]
        }];
        Text = "SpeedTool -- Game Editor";
    }

    public GameEditorWindow(Game game) : this()
    {
        Name = game.Name;
        ExeName = game.ExeName;
        timingMethod = game.DefaultTimingMethod;
        script = game.Script;
        categories = new EditableCategory[game.GetCategories().Length];
        for(int i = 0; i < categories.Length; i++)
        {
            categories[i] = new EditableCategory(){
                Name = game.GetCategories()[i].Name,
                Splits = game.GetCategories()[i].Splits
            };
        }
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ImGui.PushFont(GetFont("UI"));

        TabCount = 0;

        ImGui.InputText("Game Name", ref Name, 255);
        ImGui.InputText("Exe Name", ref ExeName, 255);
        if(ExeName != "")
        {
            if(ImGui.Button("Load script"))
            {
                FileUtil.OpenFile(file => script = File.ReadAllText(file));
            }
        }
        ImGuiExtensions.TimingMethodSelector("Default timimng method", ref timingMethod);
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
        if(ImGui.Button("Test save"))
        {
            FileUtil.SaveFile(fileName => CollectGame().SaveToFile(fileName));
        }

        // Temporary stuff to debug this mess. Probably will be removed later
#if DEBUG
        DoDebugUI();
#endif

        ImGui.PopFont();
        ImGui.End();
    }

    protected override void OnClosing()
    {
        platform.LoadGame(CollectGame());
    }

    private Game CollectGame()
    {
        return new Game(Name, ExeName, timingMethod, script, CollectCategories());
    }

    private void DoTab(ref string name, ref Split[] splits)
    {
        if(ImGui.BeginTabItem(name + "###tabno" + TabCount.ToString()))
        {
            Split? editable = null;
            Split? parent = null;

            ImGui.InputText("Category Name", ref name, 256);
            ImGui.SameLine();
            if(ImGui.Button("Delete category"))
            {
                string tmpName = name;
                categories = categories.Where(x => x.Name != tmpName).ToArray();
            }
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
            ImGui.Separator();
            if(ImGui.MenuItem("Delete split"))
            {
                RemoveSplit(ref splits);
                if(splits.Length == 0)
                {
                    InsertAbove(ref splits);
                }
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

    private void RemoveSplit(ref Split[] splits)
    {
        if(popupSplitParent != null)
        {
            popupSplitParent.Subsplits = popupSplitParent.Subsplits.Where(x => x != popupSplit).ToArray();
        }
        else
        {
            splits = splits.Where(x => x != popupSplit).ToArray();
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

    private Category[] CollectCategories()
    {
        return categories.Select(x => new Category(x.Name, x.Splits)).ToArray();
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

    private TimingMethod timingMethod;

    private string Name = "";

    private string ExeName = "";

    private int TabCount = 0;

    private string script = "";

    private const string POPUP_NAME = "popup_meun";
}
