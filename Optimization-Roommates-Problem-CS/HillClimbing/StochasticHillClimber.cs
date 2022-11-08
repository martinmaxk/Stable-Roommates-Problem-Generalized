using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public class StochasticHillClimber : BaseHillClimber
{
    private double t;
    
    private int numSteps, numNeighbors;
    public override int NumSteps => numSteps;
    public override int NumNeighbors => numNeighbors;
    
    public StochasticHillClimber(double t)
    {
        this.t = t;
    }
    
    protected override void OnInitialize()
    {
        int maxGenCount = RandomNeighborGenerator.GetMaxGenBound(groups);
        neighborGenerator = new RandomNeighborGenerator(maxGenCount);
        numSteps = numNeighbors = 0;
    }

    public override bool NextStep()
    {
        numSteps++;
        objective.GetValidGroupIndices(isInGroupIndices1, isInGroupIndices2);
        neighborGenerator.Initialize(groups, isInGroupIndices1, isInGroupIndices2);
        double curValue = objective.Value;
        objective.SetNowAsRefPoint();
        numNeighbors = 0;
        while (neighborGenerator.MoveNext())
        {
            numNeighbors++;
            var neighborDelta = neighborGenerator.Current;
            ApplyDelta(neighborDelta);
            double value = objective.Value;
            if (objective.CompareToLastRefPoint() > 0 && 
                RandUtils.Probability(GetProbability(curValue, value)))
            {
                return true;
            }
            UnapplyDelta(neighborDelta);
        }
        numSteps--;
        return false;
    }

    private double GetProbability(double curValue, double newValue)
    {
        double delta = newValue - curValue;
        return 1 / (1 + Math.Exp(delta / t));
    }
}