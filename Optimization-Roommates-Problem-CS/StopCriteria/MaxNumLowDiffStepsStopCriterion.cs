namespace Optimization_Roommates_Problem_CS.StopCriteria;

public class MaxNumLowDiffStepsStopCriterion : IStopCriterion
{
    public int numLowDiffSteps, maxNumLowDiffSteps, lowObjectiveDiff;

    public MaxNumLowDiffStepsStopCriterion(int lowObjectiveDiff, int maxNumLowDiffSteps)
    {
        this.lowObjectiveDiff = lowObjectiveDiff;
        this.maxNumLowDiffSteps = maxNumLowDiffSteps;
        this.numLowDiffSteps = 0;
    }
    
    public void Reset()
    {
        this.numLowDiffSteps = 0;
    }

    public bool ShouldStop()
    {
        return numLowDiffSteps >= maxNumLowDiffSteps;
    }

    public void NextStep(double curValue, double nextValue, bool accepted)
    {
        double objectiveDiff = curValue - nextValue;
        if (objectiveDiff <= lowObjectiveDiff)
            numLowDiffSteps++;
        else
            numLowDiffSteps = 0;
    }

    public void OnTemperatureChange()
    {
    }
}
