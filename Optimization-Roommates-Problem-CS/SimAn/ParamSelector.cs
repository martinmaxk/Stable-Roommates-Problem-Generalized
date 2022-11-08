using System.Collections;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.SimAn;

public static class ParamSelector
{
    [ThreadStatic] 
    private static RandomNeighborGenerator neighborGenerator;
    private static RandomNeighborGenerator NeighborGenerator
    {
        get
        {
            if (neighborGenerator == null)
                neighborGenerator = new RandomNeighborGenerator();
            return neighborGenerator;
        }
    }
    [ThreadStatic] 
    private static BitArray isInGroupIndices1, isInGroupIndices2;

    public static double CalcBenAmeurInitialTemperature(IObjective objective, int[][] groups,
        int[][] weights, List<Transition> transitions, double x0)
    {
        int n = weights.Length;
        int k = groups.Length;
        //int targetChunkSize = (int)Math.Ceiling(Math.Log(n));
        //int numStates = (int)Math.Ceiling(Math.Log(n));
        int targetChunkSize = 5;
        int initTransCount = 10;
        int numStates = targetChunkSize * 100;
        var groupsCopy = ArrayUtils.Clone(groups);
        double logX0 = Math.Log(x0);
        const double TempEpsilon = 1e-1;
        const double AcceptEpsilon = 1e-2;
        double t = double.NegativeInfinity;
        double lastTemp = double.PositiveInfinity;
        SamplePositiveTransitions(objective, groupsCopy, weights, 
            transitions, initTransCount, initTransCount * 100, 1);
        if (transitions.Count < initTransCount)
            return objective.MaxDeltaValue(n, k);
        int numChunks = 0;
        while (Math.Abs(t - lastTemp) >= TempEpsilon)
        {
            lastTemp = t;
            // Does not converge, so use default initial temperature
            if (numChunks >= n)
                return objective.MaxDeltaValue(n, k);
            int prevNumSamples = transitions.Count;
            SamplePositiveTransitions(objective, groupsCopy, weights, 
                transitions, targetChunkSize, numStates, 1);
            int curChunkSize = transitions.Count - prevNumSamples;
            // Cannot find enough positive transitions
            if (curChunkSize < targetChunkSize)
                return objective.MaxDeltaValue(n, k);
            
            double tNum = 0;
            for (int i = 0; i < transitions.Count; i++)
                tNum += transitions[i].DeltaValue;
            t = -tNum / (transitions.Count * logX0);
            while (true)
            {
                double xEst = EstimateExpectAcceptRatioSampleSize1(transitions, t);
                // Converged towards desired accept ratio?
                if (Math.Abs(xEst - x0) <= AcceptEpsilon)
                    break;
                // p = 2, means power 1/2 = sqrt
                t *= Math.Sqrt(Math.Log(xEst) / logX0);
            }
            numChunks++;
        }
        return t;
    }

    // Estimate of expected acceptance ratio from sample
    private static double EstimateExpectAcceptRatio(List<Transition> transitions, double t)
    {
        double acceptRatioNum = 0, acceptRatioDenom = 0;
        // Value divided by t can be quite a big number, 
        // which means exp is incredibly small
        // SampleSize1 fixes this
        for (int i = 0; i < transitions.Count; i++)
        {
            var transition = transitions[i];
            acceptRatioNum += Math.Exp(-(transition.toValue / t));
            acceptRatioDenom += Math.Exp(-(transition.fromValue / t));
        }
        return acceptRatioNum / acceptRatioDenom;
    }
    
    // Calculate EstimateExpectAcceptRatio for each individual sample and returns the mean
    // This function is in theory mathematically equivalent to EstimateExpectAcceptRatio
    private static double EstimateExpectAcceptRatioSampleSize1(
        List<Transition> transitions, double t)
    {
        double acceptRatioSum = 0;
        for (int i = 0; i < transitions.Count; i++)
        {
            var transition = transitions[i];
            acceptRatioSum += Math.Exp((transition.fromValue - transition.toValue) / t);
        }
        return acceptRatioSum / transitions.Count;
    }
    
    private static void SamplePositiveTransitions(IObjective objective, int[][] groups, 
        int[][] weights, List<Transition> transitions, int numTrans, int numStates, int numTransPerState)
    {
        ArrayUtils.CreateOrResize(ref isInGroupIndices1, groups.Length, true);
        ArrayUtils.CreateOrResize(ref isInGroupIndices2, groups.Length, true);
        for (int i = 0; i < numStates; i++)
        {
            RoommatesUtils.RandomGroups(groups);
            objective.Initialize(groups, weights);
            objective.GetValidGroupIndices(isInGroupIndices1, isInGroupIndices2);
            NeighborGenerator.MaxGenCount = numTransPerState;
            NeighborGenerator.Initialize(groups);
            double fromValue = objective.Value;
            while (NeighborGenerator.MoveNext())
            {
                var neighborDelta = NeighborGenerator.Current;
                objective.ApplyDelta(groups, weights, neighborDelta);
                neighborDelta.Apply(groups);
                // Check if transition is positive
                if (fromValue + ObjectiveUtils.Epsilon < objective.Value)
                {
                    numTrans--;
                    transitions.Add(new Transition(fromValue, objective.Value));
                }
                objective.UnapplyDelta(groups, weights, neighborDelta);
                neighborDelta.Unapply(groups);

                if (numTrans <= 0)
                    return;
                i++;
            }
        }
    }
}

public struct Transition
{
    public double fromValue, toValue;
    public double DeltaValue => toValue - fromValue;

    public Transition(double fromValue, double toValue)
    {
        this.fromValue = fromValue;
        this.toValue = toValue;
    }
}