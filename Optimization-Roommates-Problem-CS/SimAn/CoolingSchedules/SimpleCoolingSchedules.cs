namespace Optimization_Roommates_Problem_CS.SimAn.CoolingSchedules;

public class LogarithmicCoolingSchedule : ICoolingSchedule
{
    // Cooling rate
    private double a;
    
    public LogarithmicCoolingSchedule(double a)
    {
        this.a = a;
    }
    
    public double GetTemperature(double t0, int k)
    {
        return (a * t0) / Math.Log(1 + k);
    }
}

public class LinearCoolingSchedule : ICoolingSchedule
{
    // Cooling rate
    private double a;
    
    public LinearCoolingSchedule(double a)
    {
        this.a = a;
    }
    
    public double GetTemperature(double t0, int k)
    {
        return t0 - (a * k);
    }
}

public class GeometricCoolingSchedule : ICoolingSchedule
{
    // Cooling rate
    private double a;
    
    public GeometricCoolingSchedule(double a)
    {
        this.a = a;
    }
    
    public double GetTemperature(double t0, int k)
    {
        return t0 * Math.Pow(a, k);
    }
}

public class ExponentialCoolingSchedule : ICoolingSchedule
{
    // Cooling rate
    private double a;
    
    public ExponentialCoolingSchedule(double a)
    {
        this.a = a;
    }
    
    public double GetTemperature(double t0, int k)
    {
        return t0 * Math.Exp(-a * k);
    }
}
