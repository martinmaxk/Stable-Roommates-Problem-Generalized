using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.StopCriteria;

public class MinPercentStopCriterion : IStopCriterion
{
    private double minRate;
    private int maxNumMinPercent;

    private int curNumMinPercent;
    private int numReps, numAccepted;
    private double curMinValue;
    
    public MinPercentStopCriterion(double minPercent = 2, int maxNumMinPercent = 5)
    {
        this.minRate = minPercent * 0.01f;
        this.maxNumMinPercent = maxNumMinPercent;
        Reset();
    }
    
    public void Reset()
    {
        curMinValue = double.PositiveInfinity;
        curNumMinPercent = 0;
        numReps = numAccepted = 0;
    }

    public bool ShouldStop()
    {
        return curNumMinPercent >= maxNumMinPercent;
    }

    public void NextStep(double curValue, double nextValue, bool accepted)
    {
        numReps++;
        if (accepted && !NumericUtils.IsClose(curValue, nextValue, ObjectiveUtils.Epsilon))
        {
            if (nextValue + ObjectiveUtils.Epsilon < curMinValue)
            {
                curNumMinPercent = 0;
                curMinValue = nextValue;
            }
            numAccepted++;
        }
    }

    public void OnTemperatureChange()
    {
        if (numAccepted / (double)numReps < minRate)
            curNumMinPercent++;
        numAccepted = numReps = 0;
    }
}