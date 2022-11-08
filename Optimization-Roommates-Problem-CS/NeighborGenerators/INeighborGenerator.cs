using System.Collections;

namespace Optimization_Roommates_Problem_CS.NeighborGenerators;

public interface INeighborGenerator : IEnumerator<NeighborDelta>
{
    bool IsInitialized { get; }
    
    /// <summary>
    /// Initialize using groups and valid group indices used in deltas
    /// </summary>
    /// <param name="isInValidGroupIndices1">Valid group indices bitmask used in from swap</param>
    /// <param name="isInValidGroupIndices2">Valid group indices bitmask used in to swap</param>
    void Initialize(int[][] groups, BitArray isInValidGroupIndices1,
        BitArray isInValidGroupIndices2);
}
