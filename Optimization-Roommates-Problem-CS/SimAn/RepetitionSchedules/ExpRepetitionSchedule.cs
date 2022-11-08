namespace Optimization_Roommates_Problem_CS.SimAn.RepetitionSchedules;

/// <summary>
/// Selects repetition as exp(d / t)
/// </summary>
public class ExpRepetitionSchedule : IRepetitionSchedule
{
    private double d;
    
    /// <summary>
    /// Construct new instance
    /// </summary>
    /// <param name="d">An estimation of the jump needed to
    /// escape from any local minimum</param>
    public ExpRepetitionSchedule(double d)
    {
        this.d = d;
    }
    
    public int GetNumRepetitions(double t, int k)
    {
        return (int)Math.Ceiling(Math.Exp(d / t));
    }
}