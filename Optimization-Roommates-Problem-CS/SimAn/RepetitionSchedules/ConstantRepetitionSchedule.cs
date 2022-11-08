namespace Optimization_Roommates_Problem_CS.SimAn.RepetitionSchedules;

public class ConstantRepetitionSchedule : IRepetitionSchedule
{
    private int numReps;
    
    public ConstantRepetitionSchedule(int numReps = 1)
    {
        this.numReps = numReps;
    }

    public int GetNumRepetitions(double t, int k)
    {
        return numReps;
    }
}