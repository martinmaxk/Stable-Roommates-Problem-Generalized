using System.Collections;
using System.Text;

namespace Optimization_Roommates_Problem_CS.Utils;

public static class StringUtils
{
    [ThreadStatic]
    private static StringBuilder builder;
    private static StringBuilder Builder
    {
        get
        {
            if (builder == null)
                builder = new StringBuilder();
            return StringUtils.builder;
        }
    }

    public static string ToString<T>(IList<T> list)
    {
        Builder.Clear();
        Builder.Append('[');
        for (int i = 0; i < list.Count; i++)
        {
            Builder.Append(list[i]);
            if (i != list.Count - 1)
                Builder.Append(", ");
        }
        Builder.Append("]\n");
        return Builder.ToString();
    }
    
    public static string ToString(BitArray bitArray)
    {
        Builder.Clear();
        Builder.Append('[');
        for (int i = 0; i < bitArray.Count; i++)
        {
            Builder.Append(bitArray[i] ? 1 : 0);
            if (i != bitArray.Count - 1)
                Builder.Append(", ");
        }
        Builder.Append(']');
        return Builder.ToString();
    }
    
    public static string ToString<T>(T[][] array, string prefix = "")
    {
        Builder.Clear();
        for (int i = 0; i < array.Length; i++)
        {
            Builder.Append(prefix);
            Builder.Append(i);
            Builder.Append(" -> ");
            var inner = array[i];
            for (int j = 0; j < inner.Length; j++)
            {
                Builder.Append(inner[j]);
                if (j != inner.Length - 1)
                    Builder.Append(' ');
            }
            Builder.Append('\n');
        }
        return Builder.ToString();
    }

    public static string GroupsToString(int[][] groups)
    {
        return ToString(groups, "Groups ");
    }
    
    public static string WeightsToString(int[][] weights)
    {
        return ToString(weights, "Weights for ");
    }
}