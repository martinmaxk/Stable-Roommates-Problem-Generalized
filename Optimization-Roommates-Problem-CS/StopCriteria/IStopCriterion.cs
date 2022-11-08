namespace Optimization_Roommates_Problem_CS.StopCriteria;

public interface IStopCriterion
{
    void Reset();
    
    bool ShouldStop();
    
    /// <summary>
    /// Updates the stop criterion to the next neighbor
    /// </summary>
    /// <param name="curValue">Value of current state</param>
    /// <param name="nextValue">Value of neighbor state</param>
    /// <param name="accepted">Was the neighbor state accepted?</param>
    void NextStep(double curValue, double nextValue, bool accepted);

    void OnTemperatureChange();
}