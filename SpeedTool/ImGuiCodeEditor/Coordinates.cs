namespace ImGuiCodeEditor;

// Represents a character coordinate from the user's point of view,
// i. e. consider an uniform grid (assuming fixed-width font) on the
// screen as it is rendered, and each cell has its own coordinate, starting from 0.
// Tabs are counted as [1..mTabSize] count empty spaces, depending on
// how many space is necessary to reach the next tab stop.
// For example, coordinate (1, 5) represents the character 'B' in a line "\tABC", when mTabSize = 4,
// because it is rendered as "    ABC" on the screen.
public class Coordinates
{
    public int mLine = 0, mColumn = 0;
    public Coordinates() { }
    public Coordinates(int aLine, int aColumn)
    {
        mLine = aLine;
        mColumn = aColumn;
        // assert(aLine >= 0);
        // assert(aColumn >= 0);
    }
    static Coordinates Invalid() { return new Coordinates(-1, -1); }

    public static bool operator ==(Coordinates a, Coordinates o)
    {
        return
            a.mLine == o.mLine &&
            a.mColumn == o.mColumn;
    }

    public static bool operator !=(Coordinates a, Coordinates o)
    {
        return
            a.mLine != o.mLine ||
            a.mColumn != o.mColumn;
    }

    public static bool operator <(Coordinates a, Coordinates o)
    {
        if (a.mLine != o.mLine)
            return a.mLine < o.mLine;
        return a.mColumn < o.mColumn;
    }

    public static bool operator >(Coordinates a, Coordinates o)
    {
        if (a.mLine != o.mLine)
            return a.mLine > o.mLine;
        return a.mColumn > o.mColumn;
    }

    public static bool operator <=(Coordinates a, Coordinates o)
    {
        if (a.mLine != o.mLine)
            return a.mLine < o.mLine;
        return a.mColumn <= o.mColumn;
    }

    public static bool operator >=(Coordinates a, Coordinates o)
    {
        if (a.mLine != o.mLine)
            return a.mLine > o.mLine;
        return a.mColumn >= o.mColumn;
    }

    public override bool Equals(object? obj)
    {
        if(obj == null)
            return false;
        Coordinates? a = (Coordinates?)obj!;
        return a == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}