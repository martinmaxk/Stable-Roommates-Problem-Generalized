using Optimization_Roommates_Problem_CS.Objectives;

namespace Optimization_Roommates_Problem_CS.Utils;

public static class RoommatesUtils
{
    [ThreadStatic]
    private static int[] tempNumbers;

    /// <summary>
    /// Generates a uniformly random n x n matrix W
    /// where W_ij can be 1 to n where i /= j
    /// </summary>
    /// <param name="n">Number of people</param>
    public static int[][] RandomWeights(int n)
    {
        var weights = ArrayUtils.Create<int>(n, n);
        RandomWeights(weights);
        return weights;
    }
    
    public static void RandomWeights(int[][] weights)
    {
        int n = weights.Length;
        for (int i = 0; i < weights.Length; i++)
        {
            var weightsFromI = weights[i];
            weightsFromI[i] = 0;
            // Add all other persons
            for (int j = 0; j < i; j++)
                weightsFromI[j] = RandUtils.Random.Next(1, n + 1);
            for (int j = i + 1; j < n; j++)
                weightsFromI[j] = RandUtils.Random.Next(1, n + 1);
        }
    }

    /// <summary>
    /// Generates k uniformly random groups of size n/k
    /// Assumes n is divisible by k
    /// </summary>
    /// <param name="n">Number of people in total</param>
    /// <param name="k">Number of groups</param>
    /// <returns>Groups</returns>
    public static int[][] RandomGroups(int n, int k)
    {
        var groups = ArrayUtils.Create<int>(k, n / k);
        RandomGroups(groups);
        return groups;
    }
    
    public static void RandomGroups(int[][] groups)
    {
        int n = ArrayUtils.Count(groups);   
        ArrayUtils.TryDoubleOrSetSize(ref tempNumbers, n);
        NumericUtils.FillRange(tempNumbers, n);
        tempNumbers.Shuffle(n);
        
        int headCount = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            for (int j = 0; j < group.Length; j++)
            {
                group[j] = tempNumbers[headCount];
                headCount++;
            }
            groups[i] = group;
        }
    }

    /// <summary>
    /// Constructs groups by adding the best candidate to a group
    /// until the groups are full. The best candidate gives
    /// the best objective value compared to the others at that time
    /// </summary>
    public static void GreedyGroups(IObjective objective, 
        int[][] groups, int[][] weights)
    {
        int n = ArrayUtils.Count(groups);
        ArrayUtils.TryDoubleOrSetSize(ref tempNumbers, n);
        NumericUtils.FillRange(tempNumbers, n);
        
        objective.Initialize(weights.Length, groups.Length);

        int curN = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            group[0] = tempNumbers[curN++];
            for (int j = 1; j < group.Length; j++)
            {
                int bestMemberIndex = BestMemberCandidateIndex(objective, 
                    i, group, j, weights, tempNumbers, curN, n);
                group[j] = tempNumbers[bestMemberIndex];
                // Remove candidate
                tempNumbers.Swap(bestMemberIndex, curN++);
            }
        }
    }

    private static int BestMemberCandidateIndex(IObjective objective, 
        int groupIndex, int[] group, int groupSize, int[][] weights, 
        int[] candidates, int startIndex, int count)
    {
        int bestCandidateIndex = -1;
        double bestValue = double.PositiveInfinity;
        for (int i = startIndex; i < count; i++)
        {
            int candidate = candidates[i];
            objective.AddMember(groupIndex, group, groupSize, weights, 
                groupSize, candidate);
            double value = objective.Value;
            if (value + ObjectiveUtils.Epsilon < bestValue)
            {
                bestCandidateIndex = i;
                bestValue = value;
            }
            objective.RemoveMember(group, groupSize, weights, 
                groupSize, candidate);
        }
        objective.AddMember(groupIndex, group, groupSize, weights, 
            groupSize, candidates[bestCandidateIndex]);
        return bestCandidateIndex;
    }

    /// <summary>
    /// Calculate the number of pairs of people that can be swapped
    /// i.e. number of pairs of person a and b
    /// where a and b are in different groups.
    /// n * (n - (n / k)) when all groups are the same size
    /// </summary>
    public static int CountNumSwapPairs(int[][] groups)
    {
        int n = ArrayUtils.Count(groups);
        int numPairs = 0;
        for (int i = 0; i < groups.Length; i++)
            numPairs += n - groups[i].Length;
        return numPairs;
    }
    
    public static int CountNumSwapPairs(int n, int k)
    {
        return (n * (n - (n / k))) / 2;
    }
}
