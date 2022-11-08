namespace Optimization_Roommates_Problem_CS.SimAn.RepetitionSchedules;

public interface IRepetitionSchedule
{
    /// <summary>
    /// Returns number of repetitions at temperature t_k
    /// </summary>
    /// <param name="t">Temperature at iteration k</param>
    /// <param name="k">Current iteration</param>
    int GetNumRepetitions(double t, int k);
}
