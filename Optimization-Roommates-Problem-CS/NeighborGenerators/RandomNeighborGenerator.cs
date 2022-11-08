using System.Collections;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.NeighborGenerators;

/// <summary>
/// Generates up to maxGenCount random neighbors,
/// may generate the same neighbor more than once
/// </summary>
public class RandomNeighborGenerator : INeighborGenerator
{
    private Random Random => RandUtils.Random;
    private int curGenCount;
    public int? MaxGenCount;
    private int[][] groups;
    private List<int> validGroupIndices1 = new List<int>(), 
        validGroupIndices2 = new List<int>();
    public NeighborDelta Current { get; private set; }

    object IEnumerator.Current => Current;
    
    public bool IsInitialized => groups != null;

    public RandomNeighborGenerator(int? maxGenCount = null)
    {
        this.MaxGenCount = maxGenCount;
    }
    
    public void Initialize(int[][] groups)
    {
        var isInValidGroupIndices1 = new BitArray(groups.Length);
        var isInValidGroupIndices2 = new BitArray(groups.Length);
        isInValidGroupIndices1.SetAll(true);
        isInValidGroupIndices2.SetAll(true);
        Initialize(groups, isInValidGroupIndices1, isInValidGroupIndices2);
    }
    
    public void Initialize(int[][] groups, BitArray isInValidGroupIndices1, 
        BitArray isInValidGroupIndices2)
    {
        this.groups = groups;
        NumericUtils.BitmaskToRange(isInValidGroupIndices1, validGroupIndices1);
        NumericUtils.BitmaskToRange(isInValidGroupIndices2, validGroupIndices2);
        Reset();
    }
    
    public bool MoveNext()
    {
        Assert.IsTrue(IsInitialized);
        if (curGenCount >= MaxGenCount)
            return false;

        int groupIndex1 = Random.Next(validGroupIndices1.Count);
        int groupIndex2 = Random.Next(validGroupIndices2.Count);
        // Do not swap two members of the same group
        if (validGroupIndices2[groupIndex2] == validGroupIndices1[groupIndex1])
        {
            // Covers the case where one of validGroupIndices has size 1
            if (validGroupIndices1.Count > validGroupIndices2.Count)
            {
                groupIndex1++;
                if (groupIndex1 == validGroupIndices1.Count)
                    groupIndex1 = 0;
            } 
            else
            {
                groupIndex2++;
                if (groupIndex2 == validGroupIndices2.Count)
                    groupIndex2 = 0;   
            }
        }
        groupIndex1 = validGroupIndices1[groupIndex1];
        groupIndex2 = validGroupIndices2[groupIndex2];
        Assert.IsTrue(groupIndex1 != groupIndex2);
        Current = new NeighborDelta(groupIndex1, 
            Random.Next(groups[groupIndex1].Length),
            groupIndex2, Random.Next(groups[groupIndex2].Length));
        curGenCount++;
        return true;
    }

    public void Reset()
    {
        Current = new NeighborDelta(-1, -1, -1, -1);
        curGenCount = 0;
    }

    public void Dispose()
    {
        groups = null;
    }

    /// <summary>
    /// Get maxGenCount necessary to have
    /// one or more unexplored neighbor states with
    /// probability at most m^{-2} where m is total
    /// number of neighbors
    /// </summary>
    public static int GetMaxGenBound(int[][] groups)
    {
        int numPairs = RoommatesUtils.CountNumSwapPairs(groups);
        return (int)Math.Ceiling(3 * numPairs * Math.Log(numPairs));
    }
}