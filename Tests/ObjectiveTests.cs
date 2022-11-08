using System.Collections;
using Optimization_Roommates_Problem_CS;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Tests;

public class ObjectiveTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private static Random random = new Random(0);
    
    public ObjectiveTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void IndividualSimpleTest()
    {
        var weights = RoommatesUtils.RandomWeights(4);
        weights[3] = new[] { 1, 2, 3, 0 };
        var group = new int[] { 1, 2, 3 };
        long actualIndividualObj = ObjectiveUtils.Individual(group, weights[3], 2);
        Assert.Equal(2 + 3, actualIndividualObj);
    }

    [Fact]
    public void UtilCalculateSimpleTest()
    {
        GetSimpleGroupsAndWeights(out var groups, out var weights);
        long actualObjective = UtilObjective.Calculate(groups, weights);
        Assert.Equal(1 + 1 + 3 + 3, actualObjective);
    }

    [Fact]
    public void DeonCalculateSimpleTest()
    {
        GetSimpleGroupsAndWeights(out var groups, out var weights);
        long actualObjective = DeonObjective.Calculate(groups, weights);
        // 2 and 3 have it worst (has each other ranked at 3)
        Assert.Equal(3, actualObjective);
    }

    [Fact]
    public void DeonTieCompareToLastRefPointTest()
    {
        DeonGetTieGroupsAndWeights(out var groups, out var weights);
        var objective = new DeonObjective();
        objective.Initialize(groups, weights);
        double prevValue = objective.Value;
        var neighborDelta = new NeighborDelta(0, 0, 1, 0);
        var bitmask = new BitArray(groups.Length, true);
        var bitmask2 = new BitArray(groups.Length, true);
        objective.GetValidGroupIndices(bitmask, bitmask2);
        Assert.Equal(new BitArray(new[] { true, true, true }), bitmask);
        Assert.Equal(new BitArray(new[] { true, true, true }), bitmask2);
        objective.SetNowAsRefPoint();
        // No change, should be equal
        Assert.Equal(0, objective.CompareToLastRefPoint());
        // Change to better tie
        objective.ApplyDelta(groups, weights, neighborDelta);
        neighborDelta.Apply(groups);
        objective.GetValidGroupIndices(bitmask, bitmask2);
        Assert.Equal(new BitArray(new[] { true, true, true }), bitmask);
        Assert.Equal(new BitArray(new[] { false, false, true }), bitmask2);
        Assert.Equal(prevValue, objective.Value, 4);
        Assert.Equal(1, objective.CompareToLastRefPoint());
        objective.SetNowAsRefPoint();
        Assert.Equal(0, objective.CompareToLastRefPoint());
        // Change back to worse tie
        objective.UnapplyDelta(groups, weights, neighborDelta);
        neighborDelta.Unapply(groups);
        objective.GetValidGroupIndices(bitmask, bitmask2);
        Assert.Equal(new BitArray(new[] { true, true, true }), bitmask);
        Assert.Equal(new BitArray(new[] { true, true, true }), bitmask2);
        Assert.Equal(-1, objective.CompareToLastRefPoint());
    }

    private static void DeonGetTieGroupsAndWeights(out int[][] groups, out int[][] weights)
    {
        groups = new int[][] { new[] { 0, 1 }, new[] { 2, 3 }, new[] { 4, 5 } };
        weights = new int[][]
        {
            new[] { 0, 5, 2, 3, 4, 1 }, new[] { 5, 0, 2, 3, 4, 1 },
            new[] { 1, 2, 0, 5, 3, 4 }, new[] { 1, 2, 5, 0, 4, 3 },
            new[] { 1, 2, 3, 4, 0, 5 }, new[] { 1, 2, 3, 4, 5, 0 }
        };
    }
    
    [Theory]
    [InlineData(3, 0)]
    [InlineData(1 + 1 + 3 + 3, 1)]
    [InlineData(((1 + 1 + 3 + 3) * 0.5) + (3 * 0.5), 0.5)]
    [InlineData(((1 + 1 + 3 + 3) * 0.25) + (3 * 0.75), 0.25)]
    [InlineData(((1 + 1 + 3 + 3) * 0.75) + (3 * 0.25), 0.75)]
    public void MixedCalculateSimpleTest(double expected, double utilMultiplier)
    {
        GetSimpleGroupsAndWeights(out var groups, out var weights);
        double actualObjective = MixedObjective.Calculate(groups, weights, utilMultiplier);
        // Util and deon combined
        Assert.Equal(expected, actualObjective, 4);
    }

    public static IEnumerable<object[]> AllObjectivesGen()
    {
        var objectives = new object[]
        {
            new UtilObjective(), new DeonObjective(), new MixedObjective(0),
            new MixedObjective(1), new MixedObjective(0.5), new MixedObjective(0.25),
            new MixedObjective(0.75)
        };
        var result = new object[objectives.Length][];
        for (int i = 0; i < objectives.Length; i++)
            result[i] = new object[] { objectives[i] };
        return result;
    }
    
    public static IEnumerable<object[]> RandomStateGen()
    {
        var objectives = AllObjectivesGen().ToArray();
        var result = new object[objectives.Length * 5][];
        int resultCount = 0;
        for (int i = 0; i < objectives.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GetRandomState(out var groups, out var weights);
                result[resultCount++] = new[] { objectives[i][0], groups, weights };
            }
        }
        return result;
    }

    [Theory]
    [MemberData(nameof(AllObjectivesGen))]
    public void RemoveThenAddMemberValueConservationTest(IObjective objective)
    {
        GetSimpleGroupsAndWeights(out var groups, out var weights);
        double originObjective = objective.CalculateSlow(groups, weights);
        objective.Initialize(groups, weights);
        objective.RemoveMember(groups[1], groups[1].Length, weights, 
            1, groups[1][1]);
        var tempGroup = groups[1];
        groups[1] = new int[] { groups[1][1] };
        double actualObjective = objective.CalculateSlow(groups, weights);
        Assert.Equal(objective.Value, actualObjective, 4);
        groups[1] = tempGroup;
        objective.AddMember(1, groups[1], groups[1].Length, weights, 
            1, groups[1][1]);
        Assert.Equal(objective.Value, originObjective, 4);
    }

    [Theory]
    [MemberData(nameof(RandomStateGen))]
    public void ApplyDeltaAndCalculateAreEqualTest(IObjective objective,
        int[][] groups, int[][] weights)
    {
        var neighborGen = new RandomNeighborGenerator();
        neighborGen.Initialize(groups);
        objective.Initialize(groups, weights);
        double expectedObjective = objective.CalculateSlow(groups, weights);
        Assert.Equal(expectedObjective, objective.Value, 4);
        neighborGen.MoveNext();
        var neighborDelta = neighborGen.Current;
        /*testOutputHelper.WriteLine(StringUtils.GroupsToString(groups));
        testOutputHelper.WriteLine(StringUtils.ToString(weights));
        testOutputHelper.WriteLine(neighborDelta.ToString());*/
        objective.ApplyDelta(groups, weights, neighborDelta);
        neighborDelta.Apply(groups);
        expectedObjective = objective.CalculateSlow(groups, weights);
        Assert.Equal(expectedObjective, objective.Value, 4);
    }

    private static void VerifyContainsRange(int[][] array, int n)
    {
        var containsNthNumber = new bool[n];
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                int num = array[i][j];
                Assert.False(containsNthNumber[num], num.ToString());
                containsNthNumber[num] = true;
            }
        }
        for (int i = 0; i < n; i++)
            Assert.True(containsNthNumber[i], i.ToString());
    }
    
    private static void GetRandomState(out int[][] groups, out int[][] weights)
    {
        int k = random.Next(2, 20);
        int n = k * random.Next(2, 10);
        groups = RoommatesUtils.RandomGroups(n, k);
        VerifyContainsRange(groups, n);
        weights = RoommatesUtils.RandomWeights(n);
    }

    public static void GetSimpleGroupsAndWeights(out int[][] groups, out int[][] weights)
    {
        groups = new int[][] { new[] { 0, 1 }, new[] { 2, 3 } };
        weights = new int[][]
        {
            new[] { 0, 1, 2, 3 }, new[] { 1, 0, 2, 3 },
            new[] { 1, 2, 0, 3 }, new[] { 1, 2, 3, 0 }
        };
    }
}