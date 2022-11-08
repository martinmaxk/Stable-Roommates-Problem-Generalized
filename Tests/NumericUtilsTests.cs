using System.Collections;
using Optimization_Roommates_Problem_CS;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Utils;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Tests;

public class NumericUtilsTests
{
    [Fact]
    public void RangeToBitmaskSimpleTest()
    {
        var range = new List<int>() { 1, 3 };
        var actual = NumericUtils.RangeToBitmask(range, 4);
        Assert.Equal(new BitArray(new[] { false, true, false, true }), actual);
    }
    
    [Fact]
    public void BitmaskToRangeSimpleTest()
    {
        var bitmask = new BitArray(new[] { false, true, false, true });
        var actual = NumericUtils.BitmaskToRange(bitmask);
        Assert.Equal(new List<int>() { 1, 3 }, actual);
    }
}