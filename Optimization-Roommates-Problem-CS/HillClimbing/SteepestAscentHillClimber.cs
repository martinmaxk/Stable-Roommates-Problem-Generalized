using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public class SteepestAscentHillClimber : BaseHillClimber
{
    private int numSteps, numNeighbors;
    public override int NumSteps => numSteps;
    public override int NumNeighbors => numNeighbors;
    
    protected override void OnInitialize()
    {
        neighborGenerator = new LexicalNeighborGenerator();
        numSteps = numNeighbors = 0;
    }

    public override bool NextStep()
    {
        objective.GetValidGroupIndices(isInGroupIndices1, isInGroupIndices2);
        neighborGenerator.Initialize(groups, isInGroupIndices1, isInGroupIndices2);
        objective.SetNowAsRefPoint();
        NeighborDelta? bestDelta = null;
        numNeighbors = 0;
        while (neighborGenerator.MoveNext())
        {
            numNeighbors++;
            var neighborDelta = neighborGenerator.Current;
            ApplyDelta(neighborDelta);
            if (objective.CompareToLastRefPoint() > 0)
            {
                objective.SetNowAsRefPoint();
                bestDelta = neighborDelta;
            }
            UnapplyDelta(neighborDelta);
        }
        if (!bestDelta.HasValue)
            return false;
        ApplyDelta(bestDelta.Value);
        numSteps++;
        return true;
    }
}