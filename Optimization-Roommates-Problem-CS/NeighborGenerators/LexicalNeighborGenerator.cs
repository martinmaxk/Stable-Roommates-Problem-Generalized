using System.Collections;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.NeighborGenerators;

public class LexicalNeighborGenerator : INeighborGenerator
{
    private int[][] groups;
    private int validGroupIndex1, validGroupIndex2;
    private NeighborDelta current;
    public NeighborDelta Current => current;
    object IEnumerator.Current => Current;

    private List<int> validGroupIndices1 = new List<int>(), 
        validGroupIndices2 = new List<int>();
    private BitArray isInValidGroupIndices1, isInValidGroupIndices2;

    #if DEBUG
    private HashSet<NeighborDelta> generated = new HashSet<NeighborDelta>();
    #endif

    public bool IsInitialized => groups != null;

    public void Initialize(int[][] groups)
    {
        ArrayUtils.CreateOrResize(ref isInValidGroupIndices1, groups.Length, true);
        ArrayUtils.CreateOrResize(ref isInValidGroupIndices2, groups.Length, true);
        Initialize(groups, isInValidGroupIndices1, isInValidGroupIndices2);
    }
    
    public void Initialize(int[][] groups, BitArray isInValidGroupIndices1, 
        BitArray isInValidGroupIndices2)
    {
        this.groups = groups;
        this.isInValidGroupIndices1 = isInValidGroupIndices1;
        this.isInValidGroupIndices2 = isInValidGroupIndices2;
        NumericUtils.BitmaskToRange(isInValidGroupIndices1, validGroupIndices1);
        NumericUtils.BitmaskToRange(isInValidGroupIndices2, validGroupIndices2);
        Reset();
    }

    public bool MoveNext()
    {
        Assert.IsTrue(IsInitialized);
        if (!TrySetNextNeighborDelta())
            return false;

        #if DEBUG
        Assert.IsTrue(!generated.Contains(Current));
        generated.Add(Current);
        #endif
        return true;

    }

    public void Reset()
    {
        validGroupIndex1 = 0;
        validGroupIndex2 = 0;
        current = new NeighborDelta(validGroupIndices1[validGroupIndex1], 0, 
            validGroupIndices2[validGroupIndex2], -1);
        #if DEBUG
        generated.Clear();
        #endif
    }

    public void Dispose()
    {
        groups = null;
        validGroupIndices1 = validGroupIndices2 = null;
    }
    
    private bool TrySetNextNeighborDelta()
    {
        while (TryGetNextPersonIndex(ref validGroupIndex2,
                ref current.memberIndex2, validGroupIndices2.Count))
        {
            current.groupIndex2 = validGroupIndices2[validGroupIndex2];
            if (current.groupIndex2 == current.groupIndex1)
                continue;
            // We already generated swap pairs among these two groups
            // If earlier groupIndex1 was groupIndex2 and vice-versa
            if (current.groupIndex2 < current.groupIndex1 &&
                isInValidGroupIndices1.Get(current.groupIndex2) &&
                isInValidGroupIndices2.Get(current.groupIndex1))
                continue;
            return true;
        }

        if (TryGetNextPersonIndex(ref validGroupIndex1,
                ref current.memberIndex1, validGroupIndices1.Count))
        {
            validGroupIndex2 = 0;
            current.memberIndex2 = -1;
            current.groupIndex1 = validGroupIndices1[validGroupIndex1];
            current.groupIndex2 = validGroupIndices2[validGroupIndex2];
            return TrySetNextNeighborDelta();
        }

        return false;
    }

    private bool TryGetNextPersonIndex(ref int groupIndex, ref int memberIndex,
        int groupCount)
    {
        if (memberIndex + 1 < groups[groupIndex].Length)
        {
            memberIndex++;
            return true;
        }
        if (groupIndex + 1 < groupCount)
        {
            groupIndex++;
            memberIndex = 0;
            return true;
        }
        return false;
    }
}
