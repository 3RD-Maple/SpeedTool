using SpeedTool.Global;

namespace SpeedTool.Platform;

using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Input;
using System.Drawing;

using SilkWindow = Silk.NET.Windowing.Window;
using Silk.NET.Maths;

using Sizes = Silk.NET.Maths.Vector2D<int>;
using ImGuiNET;
using System.Numerics;
using SpeedTool.Platform.EventsArgs;
using StbImageSharp;
using Silk.NET.Core;

public class Window : IDisposable
{
    public Window(WindowOptions options, Sizes sz)
    {
        window = SilkWindow.Create(options);
        sizes = sz;

        fonts = new();

        window.Load += () =>
        {
            gl = window.CreateOpenGL();
            gl.Enable(EnableCap.Texture2D);
            input = window.CreateInput();
            images = new Images(gl!);
            controller = new ImGuiController(gl, window, input, () => {
                LoadDefaultFont();
                LoadFontEx(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\segoeui.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\meiryo.ttc", 22, "UI");
                var stream = GetType().Assembly.GetManifestResourceStream(ICON_RESOURCE_NAME)!;
                var img = ImageResult.FromStream(stream);
                var rawImg = new RawImage(img.Width, img.Height, new(img.Data));
                window.SetWindowIcon(ref rawImg);
                OnLoad();
            });
        };

        window.Render += delta =>
        {
            controller?.Update((float)delta);
            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
            gl?.ClearColor(Color.FromArgb(0, 26, 20, 27));
            gl?.Clear((uint)ClearBufferMask.ColorBufferBit);

            OnDraw(delta);
            gl?.Flush();

            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
            controller?.MakeCurrent();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.25f, 0.22f, 0.31f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.51f, 0.32f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.643f, 0.576f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.51f, 0.32f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.25f, 0.22f, 0.31f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.51f, 0.32f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.25f, 0.22f, 0.31f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Tab, new Vector4(0.25f, 0.22f, 0.31f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabActive, new Vector4(0.51f, 0.32f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.643f, 0.576f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.643f, 0.576f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.643f, 0.576f, 1.0f, 1.0f));

            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
            OnUI(delta);
            OnAfterUI(delta);

            ImGui.PopStyleColor(12);
            ImGui.PopStyleVar();

            controller?.Render();
        };

        window.Resize += sizes =>
        {
            this.sizes = sizes;
            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
        };

        window.Closing += () =>
        {
            var args = new BeforeClosingEventArgs();
            BeforeClosing?.Invoke(this, args);
            if(!args.ShouldClose)
                return;
            OnClosing();
            input?.Dispose();
            gl?.Dispose();
            Configuration.OnConfigurationChanged -= HandleConfigUpdate;

            // FIXME:
            //  Disposing of ImGuiController leads to a catastrophic failure of some sort.
            //  I don't really know what exactly causes it, I suspect it has something to do with fonts.
            //  Further investigation is needed; Until then -- don't dispose of the ImGui controller!
            /*
                controller?.Dispose();
            */
        };

        window.FileDrop += files =>
        {
            OnFilesDropped(files);
        };

        window.Initialize();
        window.Size = sizes;

        Configuration.OnConfigurationChanged += HandleConfigUpdate;
    }

    public event EventHandler<BeforeClosingEventArgs>? BeforeClosing;

    private void HandleConfigUpdate(object? sender, IConfigurationSection section)
    {
        OnConfigUpdated(sender, section);
    }

    public Sizes Sizes
    {
        get => sizes;
        set
        {
            window.Size = value;
            sizes = value;
        }
    }

    public string Text
    {
        get
        {
            return window.Title;
        }
        set
        {
            window.Title = value;
        }
    }

    public bool IsClosed => window.IsClosing;

    public void Cycle()
    {
        window.DoEvents();
        if (!IsClosed)
        {
            window.DoUpdate();
        }
        if(!IsClosed)
        {
                window.DoRender();
        }
    }

    public virtual void Dispose()
    {
        window.Dispose();
    }

    public void Close()
    {
        window.Close();
    }

    public GL Gl
    {
        get
        {
            return gl!;
        }
    }

    public void Reset()
    {
        window.Reset();
    }

    protected Images Images
    {
        get { return images!; }
    }

    protected virtual void OnLoad() { }

    protected virtual void OnUI(double dt) { }

    protected virtual void OnDraw(double dt) { }

    protected virtual void OnAfterUI(double dt) { }

    protected virtual void OnFilesDropped(string[] files) { }

    /// <summary>
    /// Called when the window is closing
    /// </summary>
    protected virtual void OnClosing() { }

    protected virtual void OnConfigUpdated(object? sender, IConfigurationSection? section) { }

    private IWindow window;

    public unsafe void LoadFont(string font, int size, string name)
    {
        var builder = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
        builder.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesCyrillic());
        builder.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesJapanese());
        builder.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesKorean());
        builder.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesCyrillic());
        ImVector vc;
        builder.BuildRanges(out vc);
        ImFontConfigPtr config = new(ImGuiNative.ImFontConfig_ImFontConfig())
        {
            GlyphRanges = vc.Data
        };
        fonts[name] = ImGui.GetIO().Fonts.AddFontFromFileTTF(font, size, config);
    }

    public unsafe void LoadFontEx(string fontStd, string fontCJK, int size, string name)
    {
        var res = ImGui.GetIO().Fonts.AddFontFromFileTTF(fontStd, size, null, ImGui.GetIO().Fonts.GetGlyphRangesDefault());

        ImFontConfigPtr config = new(ImGuiNative.ImFontConfig_ImFontConfig());
        config.MergeMode = true;


        ImGui.GetIO().Fonts.AddFontFromFileTTF(fontStd, size, config, ImGui.GetIO().Fonts.GetGlyphRangesCyrillic());
        ImGui.GetIO().Fonts.AddFontFromFileTTF(fontCJK, size, config, ImGui.GetIO().Fonts.GetGlyphRangesJapanese());
        ImGui.GetIO().Fonts.AddFontFromFileTTF(fontCJK, size, config, ImGui.GetIO().Fonts.GetGlyphRangesKorean());
        ImGui.GetIO().Fonts.AddFontFromFileTTF(fontCJK, size, config, ImGui.GetIO().Fonts.GetGlyphRangesChineseFull());
        ImGui.GetIO().Fonts.Build();
        fonts[name] = res;
    }

    private void LoadDefaultFont()
    {
        fonts["default"] = ImGui.GetIO().Fonts.AddFontDefault();
    }

    public ImFontPtr GetFont(string name)
    {
        return fonts[name];
    }

    Dictionary<string, ImFontPtr> fonts;
    
    private GL? gl;
    private IInputContext? input;
    ImGuiController? controller;

    private Images? images;

    Vector2D<int> sizes;

    private const string ICON_RESOURCE_NAME = "SpeedTool.Resources.icon.png";
}