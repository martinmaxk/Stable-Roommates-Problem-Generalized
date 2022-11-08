using Optimization_Roommates_Problem_CS.HillClimbing;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.SimAn.CoolingSchedules;
using Optimization_Roommates_Problem_CS.SimAn.RepetitionSchedules;
using Optimization_Roommates_Problem_CS.StopCriteria;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.SimAn;

public class SimulatedAnnealing : BaseHillClimber
{
    private IStopCriterion stopCriterion;
    private IRepetitionSchedule repetitionSchedule;
    private ICoolingSchedule coolingSchedule;
    private double initialTemperature;
    public int NumCoolingSteps { get; private set; }
    
    private int numSteps, numNeighbors;
    public override int NumSteps => numSteps;
    public override int NumNeighbors => numNeighbors;
    public int NumReps { get; private set; }

    public SimulatedAnnealing(ICoolingSchedule coolingSchedule,
        double initialTemperature, IRepetitionSchedule repetitionSchedule, 
        IStopCriterion stopCriterion)
    {
        this.coolingSchedule = coolingSchedule;
        this.initialTemperature = initialTemperature;
        this.repetitionSchedule = repetitionSchedule;
        this.stopCriterion = stopCriterion;
    }
    
    protected override void OnInitialize()
    {
        //int maxGenCount = RandomNeighborGenerator.GetMaxGenBound(groups);
        neighborGenerator = new RandomNeighborGenerator();
        NumCoolingSteps = 0;
        numSteps = numNeighbors = 0;
        stopCriterion.Reset();
    }

    public override bool NextStep()
    {
        if (stopCriterion.ShouldStop())
            return false;
        numSteps++;
        objective.GetValidGroupIndices(isInGroupIndices1, isInGroupIndices2);
        neighborGenerator.Initialize(groups, isInGroupIndices1, isInGroupIndices2);
        double curValue = objective.Value;
        double t = coolingSchedule.GetTemperature(initialTemperature, NumCoolingSteps);
        int maxNumReps = repetitionSchedule.GetNumRepetitions(t, NumCoolingSteps);
        numNeighbors = 0;
        for (; NumReps < maxNumReps; NumReps++)
        {
            numNeighbors++;
            bool moveNext = neighborGenerator.MoveNext();
            Assert.IsTrue(moveNext);
            var neighborDelta = neighborGenerator.Current;
            ApplyDelta(neighborDelta);
            double value = objective.Value;
            bool accept = TryAcceptState(curValue, value, t);
            stopCriterion.NextStep(curValue, value, accept);
            if (accept)
            {
                NumReps++;
                return true;
            }
            UnapplyDelta(neighborDelta);
        }
        NumReps = 0;
        NumCoolingSteps++;
        stopCriterion.OnTemperatureChange();
        return true;
    }

    private bool TryAcceptState(double curValue, double value, double t)
    {
        if (value < curValue || 
            NumericUtils.IsClose(value, curValue, ObjectiveUtils.Epsilon))
            return true;
        if (RandUtils.Probability(GetProbability(curValue, value, t)))
            return true;
        return false;
    }

    private static double GetProbability(double curValue, double newValue, double t)
    {
        double negatedDelta = curValue - newValue;
        return Math.Exp(negatedDelta / t);
    }
}
