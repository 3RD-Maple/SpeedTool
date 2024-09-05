namespace ImGuiCodeEditor;

public struct EditorState
{
    public EditorState() { }

    public Coordinates mSelectionStart = new();
    public Coordinates mSelectionEnd = new();
    public Coordinates mCursorPosition = new();

    public void SwapEndsIfNeeded()
    {
        if (mSelectionStart > mSelectionEnd)
        {
            var tmp = mSelectionStart;
            mSelectionStart = mSelectionEnd;
            mSelectionEnd = tmp;
        }
    }
}
