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
        lang.mIdentifiers["module_base_address"] = new() { mDeclaration = "Get module's base address. Module search is case-insensitive" };
        lang.mIdentifiers["pointer_path"] = new() { mDeclaration = "Unwrap a pointer path" };
        lang.mIdentifiers["read_int"] = new() { mDeclaration = "Read a 32-bit integer" };
        lang.mIdentifiers["read_long"] = new() { mDeclaration = "Read a 64-bit integer" };
        lang.mIdentifiers["read_float"] = new() { mDeclaration = "Read a 32-bit single precision floating point" };
        lang.mIdentifiers["read_double"] = new() { mDeclaration = "Read a 64-bit double precision floating point" };
        lang.mIdentifiers["read_ascii"] = new() { mDeclaration = "Read a sequence of ASCII characters" };
        lang.mIdentifiers["read_bytes"] = new() { mDeclaration = "Read a sequence of bytes" };

        lang.mIdentifiers["timer_set_loading"] = new() { mDeclaration = "Set timer to loading" };
        lang.mIdentifiers["timer_set_not_loading"] = new() { mDeclaration = "Set timer to not loading" };
        lang.mIdentifiers["timer_start"] = new() { mDeclaration = "Start the timer. If timer is already started, does nothing " };
        lang.mIdentifiers["timer_split"] = new() { mDeclaration = "Split the timer" };

        lang.mIdentifiers["debug_message"] = new() { mDeclaration = "Send a debug message" };
        lang.mIdentifiers["debug_message_address"] = new() { mDeclaration = "Send a debug message containing an address" };
        return lang;
    }

    TextEditor textEditor = new();
    private string text = "";
}
