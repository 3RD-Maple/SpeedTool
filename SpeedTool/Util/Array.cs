namespace SpeedTool.Util;

public static class ArrayUtil
{
    public static T[] InsertAt<T>(this T[] array, int idx, T value)
    {
        return array.Take(idx).Append(value).Concat(array.TakeLast(array.Length - idx)).ToArray();
    }
}
