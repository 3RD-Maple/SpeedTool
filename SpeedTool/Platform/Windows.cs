namespace SpeedTool.Platform;

using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Input;
using ImGuiNET;
using System.Drawing;

using SilkWindow = Silk.NET.Windowing.Window;
using Silk.NET.Maths;

using Sizes = Silk.NET.Maths.Vector2D<int>;

public class Window : IDisposable
{
    public Window(WindowOptions options, Sizes sz)
    {
        window = SilkWindow.Create(options);
        sizes = sz;

        window.Load += () =>
        {
            gl = window.CreateOpenGL();
            input = window.CreateInput();
            controller = new ImGuiController(gl, window, input, () => {
                OnLoad();
            });
        };

        window.Render += delta =>
        {
            controller?.Update((float)delta);
            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
            gl?.ClearColor(Color.FromArgb(0, 0, 0, 0));
            gl?.Clear((uint)ClearBufferMask.ColorBufferBit);

            OnDraw(delta);
            gl?.Flush();

            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
            controller?.MakeCurrent();
            OnUI(delta);
            OnAfterUI(delta);

            controller?.Render();
        };

        window.Resize += sizes =>
        {
            this.sizes = sizes;
            gl?.Viewport(0, 0, (uint)sizes.X, (uint)sizes.Y);
        };

        window.Closing += () =>
        {
            controller?.Dispose();
            input?.Dispose();
            gl?.Dispose();
        };

        window.Initialize();
        window.Size = sizes;
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
        if(!window.IsClosing)
        {
            window.DoUpdate();
            window.DoRender();
        }
    }

    public virtual void Dispose()
    {
        window.Dispose();
    }

    public GL Gl
    {
        get
        {
            return gl!;
        }
    }

    protected virtual void OnLoad() { }

    protected virtual void OnUI(double dt) { }

    protected virtual void OnDraw(double dt) { }

    protected virtual void OnAfterUI(double dt) { }

    private IWindow window;
    private GL? gl;
    private IInputContext? input;
    ImGuiController? controller;

    Vector2D<int> sizes;
}