using System.Collections;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.Objectives;

public class MixedObjective : IObjective
{
    private UtilObjective utilObjective = new UtilObjective();
    private DeonObjective deonObjective = new DeonObjective();

    private double utilMultiplier;
    private double deonMultiplier => 1 - utilMultiplier;
    
    public double Value
    {
        get
        {
            Assert.IsTrue(IsInitialized);
            return (utilMultiplier * utilObjective.Value) + 
                   (deonMultiplier * deonObjective.Value);
        }
    }

    private double lastValue;

    public bool IsInitialized => utilObjective.IsInitialized && 
                                 deonObjective.IsInitialized;

    public double MinValue()
    {
        return (utilObjective.MinValue() * utilMultiplier) +
               (deonObjective.MinValue() * deonMultiplier);
    }
    
    public double MaxValue()
    {
        return (utilObjective.MaxValue() * utilMultiplier) +
               (deonObjective.MaxValue() * deonMultiplier);
    }
    
    public double MinValue(int n, int k)
    {
        return (utilObjective.MinValue(n, k) * utilMultiplier) +
               (deonObjective.MinValue(n, k) * deonMultiplier);
    }

    public double MaxValue(int n, int k)
    {
        return (utilObjective.MaxValue(n, k) * utilMultiplier) +
               (deonObjective.MaxValue(n, k) * deonMultiplier);
    }
    
    public double MaxDeltaValue(int n, int k)
    {
        return (utilObjective.MaxDeltaValue(n, k) * utilMultiplier) +
               (deonObjective.MaxDeltaValue(n, k) * deonMultiplier);
    }
    
    public int ExpectedNeighborhoodSize(int n, int k)
    {
        // Max of utilitarian and deon neighborhood size
        return RoommatesUtils.CountNumSwapPairs(n, k);
    }

    public static double Calculate(int[][] groups, int[][] weights, double utilMultiplier)
    {
        Assert.IsTrue(NumericUtils.IsBetween(utilMultiplier, 0, 1));
        return (utilMultiplier * UtilObjective.Calculate(groups, weights)) +
               ((1 - utilMultiplier) * DeonObjective.Calculate(groups, weights));
    }
    
    public double CalculateSlow(int[][] groups, int[][] weights)
    {
        return Calculate(groups, weights, utilMultiplier);
    }

    public MixedObjective(double utilMultiplier)
    {
        Assert.IsTrue(NumericUtils.IsBetween(utilMultiplier, 0, 1));
        this.utilMultiplier = utilMultiplier;
    }
    
    public void Initialize(int[][] groups, int[][] weights)
    {
        utilObjective.Initialize(groups, weights);
        deonObjective.Initialize(groups, weights);
    }

    public void Initialize(int n, int k)
    {
        utilObjective.Initialize(n, k);
        deonObjective.Initialize(n, k);
    }
    
    public void SetNowAsRefPoint()
    {
        lastValue = Value;
    }

    public int CompareToLastRefPoint()
    {
        if (NumericUtils.IsClose(Value, lastValue, ObjectiveUtils.Epsilon))
            return 0;
        if (Value < lastValue)
            return 1;
        return -1;
    }

    public void AddMember(int groupIndex, int[] group, int groupSize, int[][] weights, 
        int addIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        utilObjective.AddMember(groupIndex, group, groupSize, weights, addIndex, person);
        deonObjective.AddMember(groupIndex, group, groupSize, weights, addIndex, person);
    }
    
    public void RemoveMember(int[] group, int groupSize, int[][] weights, 
        int subIndex, int person)
    {
        Assert.IsTrue(IsInitialized);
        utilObjective.RemoveMember(group, groupSize, weights, subIndex, person);
        deonObjective.RemoveMember(group, groupSize, weights, subIndex, person);
    }

    public void GetValidGroupIndices(BitArray isInGroupIndices1, BitArray isInGroupIndices2)
    {
    }
}