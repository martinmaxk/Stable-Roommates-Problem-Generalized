using System.Collections;

namespace Optimization_Roommates_Problem_CS.Utils;

public static class ArrayUtils
{
    public static T[][] Create<T>(int rows, int cols)
    {
        var arr = new T[rows][];
        for (int i = 0; i < rows; i++)
            arr[i] = new T[cols];
        return arr;
    }
    
    public static T[][] Clone<T>(T[][] arr)
    {
        var clone = new T[arr.Length][];
        for (int i = 0; i < arr.Length; i++)
            clone[i] = (T[])arr[i].Clone();
        return clone;
    }

    /// <summary>
    /// Counts total number of elements in 2D array
    /// </summary>
    public static int Count<T>(T[][] array)
    {
        int n = 0;
        for (int i = 0; i < array.Length; i++)
            n += array[i].Length;
        return n;
    }
    
    /// <summary>
    /// If array is smaller than n then
    /// set to new array of size n or twice the current size,
    /// whichever is bigger
    /// </summary>
    /// <returns>Was a new array created?</returns>
    public static bool TryDoubleOrSetSize<T>(ref T[] array, int n)
    {
        if (array != null && array.Length >= n)
            return false;
        if (array == null)
            array = new T[n];
        else
            array = new T[Math.Max(n, array.Length * 2)];
        return true;
    }

    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }

    public static void CreateOrResize(ref BitArray bitArray, int n, bool defaultValue)
    {
        if (bitArray == null)
            bitArray = new BitArray(n, defaultValue);
        else
        {
            if (bitArray.Length < n)
                bitArray.Length = Math.Max(bitArray.Length * 2, n);
            bitArray.SetAll(defaultValue);
        }
    }
}