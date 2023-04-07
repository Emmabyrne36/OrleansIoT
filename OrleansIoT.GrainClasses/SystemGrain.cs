using Orleans;
using OrleansIoT.Core.GrainObjects;
using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.GrainClasses;

public class SystemGrain : Grain, ISystemGrain
{
    private Dictionary<long, double> _temperatures;

    public override Task OnActivateAsync()
    {
        _temperatures = new Dictionary<long, double>();
        return base.OnActivateAsync();
    }

    public Task SetTemperature(TemperatureReading temperatureReading)
    {
        if (_temperatures.ContainsKey(temperatureReading.DeviceId))
        {
            _temperatures[temperatureReading.DeviceId] = temperatureReading.Value;
        }
        else
        {
            _temperatures.Add(temperatureReading.DeviceId, temperatureReading.Value);
        }

        if (_temperatures.Values.Average() > 100)
        {
            Console.WriteLine("System temperature is high.");
        }

        return Task.CompletedTask;
    }
}