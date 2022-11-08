using Optimization_Roommates_Problem_CS;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Utils;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Tests;

public class NeighborGeneratorTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private Random random = new Random(0);
    public NeighborGeneratorTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void LexicalSimpleTest()
    {
        ObjectiveTests.GetSimpleGroupsAndWeights(out var groups, out var weights);
        var neighborGen = new LexicalNeighborGenerator();
        neighborGen.Initialize(groups);
        var actualDeltas = neighborGen.ToEnumerable().ToArray();
        var expectedDeltas = new[]
        {
            new NeighborDelta(0, 0, 1, 0),
            new NeighborDelta(0, 0, 1, 1),
            new NeighborDelta(0, 1, 1, 0),
            new NeighborDelta(0, 1, 1, 1)
        };
        Assert.Equal(expectedDeltas, actualDeltas);
    }
    
    [Fact]
    public void LexicalFromLeaveOutGroupTest()
    {
        var groups = RoommatesUtils.RandomGroups(8, 4);
        var neighborGen = new LexicalNeighborGenerator();
        var validGroupIndices1 = new List<int>();
        var validGroupIndices2 = new List<int>() { 1, 3 };
        NumericUtils.FillRange(validGroupIndices1, groups.Length);
        neighborGen.Initialize(groups, 
            NumericUtils.RangeToBitmask(validGroupIndices1, groups.Length), 
            NumericUtils.RangeToBitmask(validGroupIndices2, groups.Length));
        var actualDeltas = neighborGen.ToEnumerable().ToArray();
        var expectedDeltas = new[]
        {
            new NeighborDelta(0, 0, 1, 0),
            new NeighborDelta(0, 0, 1, 1),
            new NeighborDelta(0, 0, 3, 0),
            new NeighborDelta(0, 0, 3, 1),
            new NeighborDelta(0, 1, 1, 0),
            new NeighborDelta(0, 1, 1, 1),
            new NeighborDelta(0, 1, 3, 0),
            new NeighborDelta(0, 1, 3, 1),
            
            new NeighborDelta(1, 0, 3, 0),
            new NeighborDelta(1, 0, 3, 1),
            new NeighborDelta(1, 1, 3, 0),
            new NeighborDelta(1, 1, 3, 1),
            
            new NeighborDelta(2, 0, 1, 0),
            new NeighborDelta(2, 0, 1, 1),
            new NeighborDelta(2, 0, 3, 0),
            new NeighborDelta(2, 0, 3, 1),
            new NeighborDelta(2, 1, 1, 0),
            new NeighborDelta(2, 1, 1, 1),
            new NeighborDelta(2, 1, 3, 0),
            new NeighborDelta(2, 1, 3, 1),
        };
        Assert.Equal(expectedDeltas, actualDeltas);
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void RandomDifferentGroupIndicesTest(int k)
    {
        int n = k * random.Next(2, 5);
        var groups = RoommatesUtils.RandomGroups(n, k);
        var neighborGen = new RandomNeighborGenerator(10);
        neighborGen.Initialize(groups);
        while (neighborGen.MoveNext())
        {
            Assert.NotEqual(neighborGen.Current.groupIndex1, 
                neighborGen.Current.groupIndex2);   
        }
    }
    
    [Fact]
    public void RandomLeaveOutGroupTest()
    {
        var groups = RoommatesUtils.RandomGroups(12, 4);
        var validGroupIndices1 = new List<int>();
        var validGroupIndices2 = new List<int>() { 1, 3 };
        NumericUtils.FillRange(validGroupIndices1, groups.Length);
        var neighborGen = new RandomNeighborGenerator(20);
        neighborGen.Initialize(groups, 
            NumericUtils.RangeToBitmask(validGroupIndices1, groups.Length), 
            NumericUtils.RangeToBitmask(validGroupIndices2, groups.Length));
        while (neighborGen.MoveNext())
        {
            var current = neighborGen.Current;
            Assert.Contains(current.groupIndex1, validGroupIndices1);
            Assert.Contains(current.groupIndex2, validGroupIndices2);
            Assert.NotEqual(current.groupIndex1, current.groupIndex2);   
        }
    }
}