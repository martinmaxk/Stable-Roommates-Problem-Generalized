using System.Collections;

namespace Optimization_Roommates_Problem_CS.Objectives;

public interface IObjective
{
    double Value { get; }
    bool IsInitialized { get; }

    double MinValue();
    double MaxValue();
    double MinValue(int n, int k);
    double MaxValue(int n, int k);

    /// <summary>
    /// Max difference between values of two neighbors
    /// </summary>
    double MaxDeltaValue(int n, int k);
    
    /// <summary>
    /// Expected number of neighbors
    /// </summary>
    int ExpectedNeighborhoodSize(int n, int k);

    double CalculateSlow(int[][] groups, int[][] weights);
    
    /// <summary>
    /// Initialize and calculate objective value from scratch
    /// using initial groups
    /// </summary>
    void Initialize(int[][] groups, int[][] weights);
    
    /// <summary>
    /// Initialize with default objective value
    /// </summary>
    void Initialize(int n, int k);

    void SetNowAsRefPoint();
    /// <summary>
    /// Compares current value and state that of last ref point
    /// to see which is preferable
    /// </summary>
    /// <returns>1 if better than ref point, 0 if same, -1 if worst</returns>
    int CompareToLastRefPoint();

    /// <summary>
    /// Update objective before group member has been added
    /// </summary>
    /// <param name="groupIndex">Index of group</param>
    /// <param name="group">Group that was modified</param>
    /// <param name="groupSize">Current size of group</param>
    /// <param name="weights">Weights of the graph</param>
    /// <param name="addIndex">Index in group where person should be added</param>
    /// <param name="person">Person to be added</param>
    void AddMember(int groupIndex, int[] group, int groupSize, int[][] weights, 
        int addIndex, int person);

    /// <summary>
    /// Update objective before group member has been removed
    /// </summary>
    /// <param name="group">Group that was modified</param>
    /// <param name="groupSize">Current size of group</param>
    /// <param name="weights">Weights of the graph</param>
    /// <param name="subIndex">Index in group of person to be removed</param>
    /// <param name="person">Person to be removed</param>
    void RemoveMember(int[] group, int groupSize, int[][] weights, 
        int subIndex, int person);

    /// <summary>
    /// Get the group indices that are valid to swap members among
    /// in ascending sorted order
    /// </summary>
    /// <param name="isInGroupIndices1">Bitmask set of group indices valid in from part of swaps.
    /// Default is true [0, ..., k]</param>
    /// <param name="isInGroupIndices2">Bitmask set of group indices valid in to part of swaps.
    /// Default is true [0, ..., k]</param>
    void GetValidGroupIndices(BitArray isInGroupIndices1, BitArray isInGroupIndices2);
}
