namespace Optimization_Roommates_Problem_CS.SimAn.CoolingSchedules;

public interface ICoolingSchedule
{
    /// <summary>
    /// Calculates the temperature at iteration k
    /// </summary>
    /// <param name="t0">Initial temperature</param>
    /// <param name="k">Cooling iteration</param>
    /// <returns>Temperature at iteration k</returns>
    double GetTemperature(double t0, int k);
}
