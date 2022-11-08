using System.Collections;

namespace Optimization_Roommates_Problem_CS.Utils;

public static class NumericUtils
{
    public static bool IsClose(double x, double y, double epsilon)
    {
        return Math.Abs(x - y) <= epsilon;
    }

    public static bool IsBetween(double x, double low, double high)
    {
        return low <= x && x <= high;
    }
    
    public static bool IsIntegral(double x, double epsilon)
    {
        return IsClose(x, Math.Round(x), epsilon);
    }

    public static double Normalize(double value, double minValue, double maxValue)
    {
        return (value - minValue) / (maxValue - minValue);
    }

    public static double Variance(this IEnumerable<double> values)
    {
        double mean = values.Average();
        return values.Average(v => Math.Pow(v - mean, 2));
    }

    public static double StandardDeviation(this IEnumerable<double> values)
    {
        return Math.Sqrt(values.Variance());
    }

    /// <summary>
    /// Combine means of two sets of numbers
    /// </summary>
    /// <param name="mean1">Mean of first set</param>
    /// <param name="elemCount1">Number of elements in first set</param>
    /// <param name="mean2">Mean of second set</param>
    /// <param name="elemCount2">Number of elements in second set</param>
    public static double CombineMeans(double mean1, int elemCount1,
        double mean2, int elemCount2)
    {
        if (elemCount1 + elemCount2 == 0)
            return 0;
        double sums = (mean1 * elemCount1) + (mean2 * elemCount2);
        return sums / (elemCount1 + elemCount2);
    }
    
    /// <summary>
    /// Fill array with 0 to count - 1
    /// </summary>
    public static void FillRange(IList<int> array, int count)
    {
        while (array.Count < count)
            array.Add(0);
        for (int i = 0; i < count; i++)
            array[i] = i;
    }

    /// <summary>
    /// Creates array [0, ..., n-1]
    /// </summary>
    public static int[] Range(int n)
    {
        var array = new int[n];
        FillRange(array, n);
        return array;
    }

    /// <summary>
    /// Convert a list that is a subset of 0 to universeSize
    /// to a bitmask where bool i indicates whether i is in the list
    /// </summary>
    public static void RangeToBitmask(IList<int> list, int universeSize,
        ref BitArray bitArray)
    {
        ArrayUtils.CreateOrResize(ref bitArray, universeSize, false);
        for (int i = 0; i < list.Count; i++)
            bitArray.Set(list[i], true);
    }
    
    public static BitArray RangeToBitmask(IList<int> list, int universeSize)
    {
        BitArray bitArray = null;
        RangeToBitmask(list, universeSize, ref bitArray);
        return bitArray;
    }
    
    public static void BitmaskToRange(BitArray bitArray, List<int> list)
    {
        list.Clear();
        for (int i = 0; i < bitArray.Count; i++)
            if (bitArray.Get(i))
                list.Add(i);
    }
    
    public static List<int> BitmaskToRange(BitArray bitArray)
    {
        var list = new List<int>();
        BitmaskToRange(bitArray, list);
        return list;
    }
}