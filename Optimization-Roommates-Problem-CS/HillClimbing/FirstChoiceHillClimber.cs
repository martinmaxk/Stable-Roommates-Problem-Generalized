using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public class FirstChoiceHillClimber : SimpleHillClimber
{
    protected override void OnInitialize()
    {
        int maxGenCount = RandomNeighborGenerator.GetMaxGenBound(groups);
        neighborGenerator = new RandomNeighborGenerator(maxGenCount);
        numSteps = numNeighbors = 0;
    }
}