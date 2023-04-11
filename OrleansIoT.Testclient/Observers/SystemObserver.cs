using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.TestClient.Observers;

public class SystemObserver : ISystemObserver
{
    public void HighTemperature(double value)
    {
        Console.WriteLine($"Observed a high system temperature: {value}");
    }
}