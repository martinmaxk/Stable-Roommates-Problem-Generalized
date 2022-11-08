using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS;


// Difference between two neighbor states,
// i.e. which two people are swapped
public struct NeighborDelta : IEquatable<NeighborDelta>
{
    public int groupIndex1, memberIndex1, groupIndex2, memberIndex2;

    public NeighborDelta(int groupIndex1, int memberIndex1, int groupIndex2,
        int memberIndex2)
    {
        this.groupIndex1 = groupIndex1;
        this.memberIndex1 = memberIndex1;
        this.groupIndex2 = groupIndex2;
        this.memberIndex2 = memberIndex2;
    }

    public void Apply(int[][] groups)
    {
        RefUtils.Swap(ref groups[groupIndex1][memberIndex1], 
            ref groups[groupIndex2][memberIndex2]);
    }

    public void Unapply(int[][] groups)
    {
        Apply(groups);
    }

    // Order of the two people to swap does not matter
    // (gx, mx, gy, my) == (gy, my, gx, mx)
    public bool Equals(NeighborDelta other)
    {
        return (groupIndex1 == other.groupIndex1 &&
               groupIndex2 == other.groupIndex2 &&
               memberIndex1 == other.memberIndex1 &&
               memberIndex2 == other.memberIndex2) 
               ||
               (groupIndex1 == other.groupIndex2 &&
                groupIndex2 == other.groupIndex1 &&
                memberIndex1 == other.memberIndex2 &&
                memberIndex2 == other.memberIndex1);
    }

    public override string ToString()
    {
        return
            $"(g{groupIndex1}, m{memberIndex1}) <-> " +
            $"(g{groupIndex2}, m{memberIndex2})";
    }

    public override bool Equals(object? obj)
    {
        return obj is NeighborDelta delta && Equals(delta);
    }

    public override int GetHashCode()
    {
        bool firstLess = groupIndex1 < groupIndex2 ||
                         (groupIndex1 == groupIndex2 && memberIndex1 < memberIndex2);
        if (firstLess)
        {
            return HashCode.Combine(groupIndex1, memberIndex1, 
                groupIndex2, memberIndex2);
        }
        return HashCode.Combine(groupIndex2, memberIndex2, 
            groupIndex1, memberIndex1);
    }

    public static bool operator ==(NeighborDelta left, NeighborDelta right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NeighborDelta left, NeighborDelta right)
    {
        return !(left == right);
    }
}