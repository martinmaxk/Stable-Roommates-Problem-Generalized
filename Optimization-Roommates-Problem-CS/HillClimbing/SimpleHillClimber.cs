using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public class SimpleHillClimber : BaseHillClimber
{
    protected int numSteps, numNeighbors;
    public override int NumSteps => numSteps;
    public override int NumNeighbors => numNeighbors;

    protected override void OnInitialize()
    {
        neighborGenerator = new LexicalNeighborGenerator();
        numSteps = numNeighbors = 0;
    }

    public override bool NextStep()
    {
        numSteps++;
        objective.GetValidGroupIndices(isInGroupIndices1, isInGroupIndices2);
        neighborGenerator.Initialize(groups, isInGroupIndices1, isInGroupIndices2);
        objective.SetNowAsRefPoint();
        numNeighbors = 0;
        while (neighborGenerator.MoveNext())
        {
            numNeighbors++;
            var neighborDelta = neighborGenerator.Current;
            ApplyDelta(neighborDelta);
            //Assert.IsTrue(NumericUtils.IsClose(value, objective.CalculateSlow(groups, weights), 1e-4));
            if (objective.CompareToLastRefPoint() > 0)
                return true;
            UnapplyDelta(neighborDelta);
        }
        numSteps--;
        return false;
    }
}