namespace Optimization_Roommates_Problem_CS.Utils;

public static class RandUtils
{
    /*private static int seed = Environment.TickCount;
    private static readonly ThreadLocal<Random> random =
        new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));*/
    
    private static readonly ThreadLocal<Random> random =
        new ThreadLocal<Random>(() => new Random(0));
    public static Random Random => RandUtils.random.Value!;

    public static bool Probability(double p)
    {
        return Random.NextDouble() <= p;
    }

    /// <summary>
    /// Opposite direction Knuth / Fisher-Yates Shuffle
    /// </summary>
    /// <param name="array">Array to be shuffled in-place</param>
    /// <param name="count">How many elements should be shuffled</param>
    public static void Shuffle<T>(this T[] array, int count)
    {
        for (int i = 0; i < count - 1; i++)
            array.Swap(i, Random.Next(i, count));
    }
    
    public static void Shuffle<T>(this T[] array)
    {
        array.Shuffle(array.Length);
    }
}