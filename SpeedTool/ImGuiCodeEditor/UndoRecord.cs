namespace ImGuiCodeEditor;

public class UndoRecord
{
    public UndoRecord() {}
    //~UndoRecord() {}

    public UndoRecord(
        string aAdded,
        Coordinates aAddedStart,
        Coordinates aAddedEnd,

        string aRemoved,
        Coordinates aRemovedStart,
        Coordinates aRemovedEnd,

        ref EditorState aBefore,
        ref EditorState aAfter)
    {
        mAdded = aAdded;
        mAddedStart = aAddedStart;
        mAddedEnd = aAddedEnd;

        mRemoved = aRemoved;
        mRemovedStart = aRemovedStart;
        mRemovedEnd = aRemovedEnd;

        mBefore = aBefore;
        mAfter = aAfter;
    }

    public void Undo(TextEditor aEditor)
    {
        if (mAdded != "")
        {
            aEditor.DeleteRange(mAddedStart, mAddedEnd);
            aEditor.Colorize(mAddedStart.mLine - 1, mAddedEnd.mLine - mAddedStart.mLine + 2);
        }

        if (mRemoved != "")
        {
            var start = mRemovedStart;
            aEditor.InsertTextAt(start, mRemoved);
            aEditor.Colorize(mRemovedStart.mLine - 1, mRemovedEnd.mLine - mRemovedStart.mLine + 2);
        }

        aEditor.mState = mBefore;
        aEditor.EnsureCursorVisible();
    }

    public void Redo(TextEditor aEditor)
    {
        if (mRemoved.Length != 0)
        {
            aEditor.DeleteRange(mRemovedStart, mRemovedEnd);
            aEditor.Colorize(mRemovedStart.mLine - 1, mRemovedEnd.mLine - mRemovedStart.mLine + 1);
        }

        if (mAdded.Length != 0)
        {
            var start = mAddedStart;
            aEditor.InsertTextAt(start, mAdded);
            aEditor.Colorize(mAddedStart.mLine - 1, mAddedEnd.mLine - mAddedStart.mLine + 1);
        }

        aEditor.mState = mAfter;
        aEditor.EnsureCursorVisible();
    }

    public string mAdded = "";
    public Coordinates mAddedStart = new();
    public Coordinates mAddedEnd = new();

    public string mRemoved = "";
    public Coordinates mRemovedStart = new();
    public Coordinates mRemovedEnd = new();

    public EditorState mBefore = new();
    public EditorState mAfter = new();
}
