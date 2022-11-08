using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;
using Assert = Xunit.Assert;

namespace Tests;

public class RoommatesUtilsTests
{
    [Fact]
    public void GreedyGroupsSimpleTest()
    {
        var objective = new MixedObjective(1);
        ObjectiveTests.GetSimpleGroupsAndWeights(out var groups, out var weights);
        RoommatesUtils.GreedyGroups(objective, groups, weights);
        var expectedGroups = new[] { new[] { 0, 1 }, new[] { 2, 3 } };
        Assert.Equal(expectedGroups, groups);
    }
}