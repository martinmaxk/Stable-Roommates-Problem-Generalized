using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.Objectives;

public static class ObjectiveUtils
{
    public const double Epsilon = 1e-4;

    public static double NormValue(this IObjective objective)
    {
        return NumericUtils.Normalize(objective.Value, objective.MinValue(),
            objective.MaxValue());
    }
    
    /// <summary>
    /// Calculates all individual objectives and puts them in result
    /// </summary>
    public static void AllIndividual(int[][] groups, int[][] weights,
        long[] result)
    {
        int n = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            for (int j = 0; j < group.Length; j++)
            {
                int person = group[j];
                long individualValue = ObjectiveUtils.Individual(group, 
                    weights[person], j);
                result[person] = individualValue;
            }
        }
    }

    /// <summary>
    /// Calculate individual satisfaction with a group
    /// </summary>
    /// <param name="group">Group person is in</param>
    /// <param name="weights">Outgoing weights from that person</param>
    /// <param name="memberIndex">Index of person in group</param>
    /// <returns></returns>
    public static long Individual(int[] group, int[] weights,
        int memberIndex)
    {
        long individualValue = 0;
        for (int i = 0; i < group.Length; i++)
        {
            if (i == memberIndex)
            {
                continue;
            }
            int person2 = group[i];
            // position of group mate in person's preferences
            individualValue += weights[person2];
        }
        return individualValue;
    }

    /// <summary>
    /// Updates value before replacing a group member with a person
    /// </summary>
    /// <param name="groupIndex">Index of group</param>
    /// <param name="group">Group to be changed</param>
    /// <param name="groupSize">Current size of group</param>
    /// <param name="weights">Weights of the graph</param>
    /// <param name="replaceIndex">Index of subPerson in group</param>
    /// <param name="subPerson">Person to be removed</param>
    /// <param name="addPerson">Person to be added</param>
    public static void ReplaceMember(this IObjective objective, 
        int groupIndex, int[] group, int groupSize, int[][] weights, int replaceIndex, 
        int subPerson, int addPerson)
    {
        Assert.IsTrue(!group.Contains(addPerson));
        objective.RemoveMember(group, groupSize, weights, replaceIndex, subPerson);
        objective.AddMember(groupIndex, group, groupSize, weights, replaceIndex, addPerson);
    }

    public static void ApplyDelta(this IObjective objective, 
        int[][] groups, int[][] weights, NeighborDelta neighborDelta)
    {
        Assert.IsTrue(neighborDelta.groupIndex1 != 
                      neighborDelta.groupIndex2);
        var group1 = groups[neighborDelta.groupIndex1];
        var group2 = groups[neighborDelta.groupIndex2];
        int person1 = group1[neighborDelta.memberIndex1];
        int person2 = group2[neighborDelta.memberIndex2];
        objective.ReplaceMember(neighborDelta.groupIndex1, group1, group1.Length, 
            weights, neighborDelta.memberIndex1, person1, person2);
        objective.ReplaceMember(neighborDelta.groupIndex2, group2, group2.Length, 
            weights, neighborDelta.memberIndex2, person2, person1);
    }
    
    public static void UnapplyDelta(this IObjective objective,
        int[][] groups, int[][] weights, NeighborDelta neighborDelta)
    {
        objective.ApplyDelta(groups, weights, neighborDelta);
    }
}
