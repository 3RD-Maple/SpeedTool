namespace ImGuiCodeEditor;

struct Breakpoint
{
    public int mLine = -1;
    public bool mEnabled = false;
    public string mCondition = "";

    public Breakpoint() { }
}