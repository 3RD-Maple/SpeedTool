using System.Numerics;
using ImGuiCodeEditor;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SpeedTool.Windows;

using Window = Platform.Window;

public sealed class CodeEditorWindow : Window
{
    public CodeEditorWindow(string text) : this()
    {
        this.text = text;
        textEditor.SetText(text);
        textEditor.LanguageDefinition = CreateLanguageDefinition();
    }

    private CodeEditorWindow() : base(options, new Vector2D<int>(1200, 780)) { }

    public string Code
    {
        get
        {
            return textEditor.GetText();
        }
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
        ImGui.Begin("CodeEditor",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoMove);

        textEditor.Render("TextEditorBlahBlah");

        ImGui.End();
    }

    private LanguageDefinition CreateLanguageDefinition()
    {
        var lang = LanguageDefinition.Lua;
        lang.mIdentifiers.Clear();
        lang.mIdentifiers["read_int"] = new() { mDeclaration = "Read a 32-bit integer" };
        lang.mIdentifiers["timer_set_loading"] = new() { mDeclaration = "Set timer to loading" };
        lang.mIdentifiers["timer_set_not_loading"] = new() { mDeclaration = "Set timer to not loading" };
        lang.mIdentifiers["debug_message"] = new() { mDeclaration = "Send a debug message" };
        return lang;
    }

    TextEditor textEditor = new();
    private string text = "";
}
