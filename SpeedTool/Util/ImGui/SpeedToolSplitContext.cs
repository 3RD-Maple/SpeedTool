using SpeedTool.Splits;

namespace SpeedTool.Util.ImGui;

public struct SpeedToolSplitContext
{
    public SpeedToolSplitContext() { }

    public bool IsHovered = false;

    public Split? selectedSplit = null;

    public Split? parentSplit = null;
}
