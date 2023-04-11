using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.TestSilo.Observers;

public class SystemObserver : ISystemObserver
{
    public void HighTemperature(double value)
    {
        Console.WriteLine($"Observed a high system temperature: {value}");
    }
}