using System.Collections;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.Objectives;

public class UtilObjective : IObjective
{
    private long value = long.MinValue;
    public double Value 
    {
        get 
        { 
            Assert.IsTrue(IsInitialized);
            return value;
        }
    }

    private double lastValue;

    public bool IsInitialized => value >= 0;

    private int n, k;

    public double MinValue() => MinValue(n, k);
    public double MaxValue() => MaxValue(n, k);

    public double MinValue(int n, int k)
    {
        return n * ((n / k) - 1);
    }

    public double MaxValue(int n, int k)
    {
        return (double)n * n * ((n / k) - 1);
    }

    public double MaxDeltaValue(int n, int k)
    {
        return 4 * (n - 1) * ((n / k) - 1);
    }

    public int ExpectedNeighborhoodSize(int n, int k)
    {
        return RoommatesUtils.CountNumSwapPairs(n, k);
    }

    public static long Calculate(int[][] groups, int[][] weights)
    {
        long value = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            for (int j = 0; j < group.Length; j++)
            {
                value += ObjectiveUtils.Individual(group, weights[group[j]], j);
            }
        }
        return value;
    }
    
    public double CalculateSlow(int[][] groups, int[][] weights)
    {
        return Calculate(groups, weights);
    }
    
    public void Initialize(int[][] groups, int[][] weights)
    {
        n = weights.Length;
        k = groups.Length;
        value = Calculate(groups, weights);
    }
    
    public void Initialize(int n, int k)
    {
        this.n = n;
        this.k = k;
        value = 0;
    }
    
    public void SetNowAsRefPoint()
    {
        lastValue = Value;
    }

    public int CompareToLastRefPoint()
    {
        if (NumericUtils.IsClose(Value, lastValue, ObjectiveUtils.Epsilon))
            return 0;
        if (Value < lastValue)
            return 1;
        return -1;
    }

    public void AddMember(int groupIndex, int[] group, int groupSize, int[][] weights, 
        int addIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        for (int i = 0; i < groupSize; i++)
        {
            if (i == addIndex)
                continue;
            int curPerson = group[i];
            value += weights[curPerson][person];
            value += weights[person][curPerson];
        }
    }
    
    public void RemoveMember(int[] group, int groupSize, int[][] weights, 
        int subIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        for (int i = 0; i < groupSize; i++)
        {
            if (i == subIndex)
                continue;
            int curPerson = group[i];
            value -= weights[curPerson][person];
            value -= weights[person][curPerson];
        }
    }

    public void GetValidGroupIndices(BitArray isInGroupIndices1, BitArray isInGroupIndices2)
    {
    }
}