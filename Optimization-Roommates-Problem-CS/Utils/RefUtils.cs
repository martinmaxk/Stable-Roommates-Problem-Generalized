namespace Optimization_Roommates_Problem_CS.Utils;

public static class RefUtils
{
    public static void Swap<T>(ref T a, ref T b)
    {
        var temp = a;
        a = b;
        b = temp;
    }
    
    public static void Swap<T>(this T[] array, int index1, int index2)
    {
        Swap(ref array[index1], ref array[index2]);
    }
}