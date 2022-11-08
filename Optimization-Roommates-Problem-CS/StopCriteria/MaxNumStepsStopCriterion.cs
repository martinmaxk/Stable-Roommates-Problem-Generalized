namespace Optimization_Roommates_Problem_CS.StopCriteria;

public class MaxNumStepsStopCriterion : IStopCriterion
{
    public int numSteps, maxNumSteps;

    public MaxNumStepsStopCriterion(int maxNumSteps)
    {
        this.maxNumSteps = maxNumSteps;
        this.numSteps = 0;
    }

    public void Reset()
    {
        this.numSteps = 0;
    }

    public bool ShouldStop()
    {
        return numSteps >= maxNumSteps;
    }

    public void NextStep(double curValue, double nextValue, bool accepted)
    {
        numSteps++;
    }
    
    public void OnTemperatureChange()
    {
    }
}
