using System.Collections;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.Objectives;

public class DeonObjective : IObjective
{
    private bool isValueDirty;
    private long[] individualObjectives;
    private int[] individualToGroupIndex;
    private int worstValueIndex = -1;
    // Number of people that has the same worst value
    private int numWorstValues;

    public double Value
    {
        get
        {
            Assert.IsTrue(IsInitialized);
            if (isValueDirty) 
                UpdateValue();
            return individualObjectives[worstValueIndex];
        }
    }
    
    private double lastValue;
    private int lastNumWorstValues;

    public bool IsInitialized => worstValueIndex >= 0;

    private int n, k;
    public double MinValue() => MinValue(n, k);
    public double MaxValue() => MaxValue(n, k);
    
    public double MinValue(int n, int k)
    {
        return (n / k) - 1;
    }

    public double MaxValue(int n, int k)
    {
        return (double)n * ((n / k) - 1);
    }
    
    public double MaxDeltaValue(int n, int k)
    {
        return MaxValue(n, k) - MinValue(n, k);
    }
    
    public int ExpectedNeighborhoodSize(int n, int k)
    {
        // Anyone in group with individual with worst sum can be swapped
        // with anyone outside his group
        // There may be multiple individuals with the same worst sum
        // For this estimate we assume there is only 1
        int groupSize = n / k;
        return groupSize * (n - groupSize);
    }
    
    /// <summary>
    /// Calculates deontological objective
    /// </summary>
    /// <param name="weights">n x n-1 matrix where
    /// (i, j) is weight from i to j</param>
    public static long Calculate(int[][] groups, int[][] weights)
    {
        long maxValue = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            for (int j = 0; j < group.Length; j++)
            {
                long individualValue =
                    ObjectiveUtils.Individual(group, weights[group[j]], j);
                maxValue = Math.Max(maxValue, individualValue);
            }
        }
        return maxValue;
    }

    public double CalculateSlow(int[][] groups, int[][] weights)
    {
        return Calculate(groups, weights);
    }
    
    public void Initialize(int[][] groups, int[][] weights)
    {
        n = weights.Length;
        k = groups.Length;
        Initialize(n, k);
        ObjectiveUtils.AllIndividual(groups, weights, individualObjectives);
        isValueDirty = true;
        for (int i = 0; i < groups.Length; i++)
            for (int j = 0; j < groups[i].Length; j++)
                individualToGroupIndex[groups[i][j]] = i;
    }
    
    public void Initialize(int n, int k)
    {
        this.n = n;
        this.k = k;
        individualObjectives = new long[n];
        worstValueIndex = 0;
        individualToGroupIndex = new int[n];
    }

    public void SetNowAsRefPoint()
    {
        lastValue = Value;
        lastNumWorstValues = numWorstValues;
    }

    public int CompareToLastRefPoint()
    {
        if (NumericUtils.IsClose(Value, lastValue, ObjectiveUtils.Epsilon))
            return lastNumWorstValues.CompareTo(numWorstValues);
        if (Value < lastValue)
            return 1;
        return -1;
    }

    public void AddMember(int groupIndex, int[] group, int groupSize, int[][] weights, 
        int addIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        int addDiff = 0;
        for (int i = 0; i < groupSize; i++)
        {
            if (i == addIndex)
                continue;
            int curPerson = group[i];
            individualObjectives[curPerson] += weights[curPerson][person];
            addDiff += weights[person][curPerson];
        }
        individualObjectives[person] += addDiff;
        individualToGroupIndex[person] = groupIndex;
        isValueDirty = true;
    }
    
    public void RemoveMember(int[] group, int groupSize, int[][] weights, 
        int subIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        int subDiff = 0;
        for (int i = 0; i < groupSize; i++)
        {
            if (i == subIndex)
                continue;
            int curPerson = group[i];
            individualObjectives[curPerson] -= weights[curPerson][person];
            subDiff -= weights[person][curPerson];
        }
        individualObjectives[person] += subDiff;
        isValueDirty = true;
    }

    public void GetValidGroupIndices(BitArray isInGroupIndices1, BitArray isInGroupIndices2)
    {
        isInGroupIndices2.SetAll(false);
        if (isValueDirty)
            UpdateValue();
        long value = individualObjectives[worstValueIndex];
        for (int i = 0; i < individualObjectives.Length; i++)
        {
            if (individualObjectives[i] == value)
                isInGroupIndices2.Set(individualToGroupIndex[i], true);
        }
    }

    private void UpdateValue()
    {
        numWorstValues = 1;
        worstValueIndex = 0;
        for (int i = 1; i < individualObjectives.Length; i++)
        {
            long curObjective = individualObjectives[i];
            long worstObjective = individualObjectives[worstValueIndex];
            if (curObjective > worstObjective)
            {
                worstValueIndex = i;
                numWorstValues = 1;
            }
            else if (curObjective == worstObjective)
                numWorstValues++;
        }
        isValueDirty = false;
    }
}
