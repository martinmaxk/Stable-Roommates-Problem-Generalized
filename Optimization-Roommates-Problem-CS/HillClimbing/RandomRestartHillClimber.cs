using Optimization_Roommates_Problem_CS.StopCriteria;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public class RandomRestartHillClimber : BaseHillClimber
{
    private BaseHillClimber hillClimber;
    public int NumRestarts { get; private set; }
    public int MaxNumRestarts { get; }
    public override int NumSteps => hillClimber.NumSteps;
    public override int NumNeighbors => hillClimber.NumNeighbors;

    private IStopCriterion stopCriterion;
    
    public double BestValue { get; private set; }
    
    public RandomRestartHillClimber(int maxNumRestarts)
    {
        this.MaxNumRestarts = maxNumRestarts;
    }

    protected override void OnInitialize()
    {
        NumRestarts = 0;
        BestValue = double.PositiveInfinity;
    }

    public bool TryRestart(BaseHillClimber hillClimber, IStopCriterion stopCriterion)
    {
        if (NumRestarts >= MaxNumRestarts)
            return false;
        this.hillClimber = hillClimber;
        this.stopCriterion = stopCriterion;
        this.stopCriterion.Reset();
        RoommatesUtils.RandomGroups(groups);
        hillClimber.Initialize(groups, weights, objective);
        BestValue = Math.Min(BestValue, Value);
        NumRestarts++;
        return true;
    }

    public override bool NextStep()
    {
        if (stopCriterion.ShouldStop())
            return false;
        double curValue = hillClimber.Value;
        if (!hillClimber.NextStep())
            return false;
        double nextValue = hillClimber.Value;
        stopCriterion.NextStep(curValue, nextValue, accepted: true);
        BestValue = Math.Min(BestValue, Value);
        return true;
    }
}
