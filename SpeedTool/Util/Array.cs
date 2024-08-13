using System.Net.NetworkInformation;

namespace SpeedTool.Util;

public static class ArrayUtil
{
    public static T[] InsertAt<T>(this T[] array, int idx, T value)
    {
        return array.Take(idx).Append(value).Concat(array.TakeLast(array.Length - idx)).ToArray();
    }

    public static IEnumerable<T> TakeAtPosWeighted<T>(this IEnumerable<T> stuff, int n, int position, int weight)
    {
        var takeRight = (int)(weight / 100.0 * n);
        takeRight = Math.Min(stuff.Count() - 1 - position, takeRight);
        var takeLeft = (int)((100 - weight) / 100.0 * n);
        if(takeLeft + takeRight < (n - 1))
            takeRight++;
        takeLeft = Math.Max(takeLeft, n - takeRight);
        takeLeft = Math.Min(takeLeft, position);
        return stuff.Skip(position - takeLeft).Take(n);
    }
}
