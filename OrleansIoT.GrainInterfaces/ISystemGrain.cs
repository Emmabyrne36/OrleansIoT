using Orleans;
using OrleansIoT.Core.GrainObjects;

namespace OrleansIoT.GrainInterfaces;

public interface ISystemGrain : IGrainWithIntegerCompoundKey
{
    Task SetTemperature(TemperatureReading temperatureReading);
    Task Subscribe(ISystemObserver observer);
    Task Unsubscribe(ISystemObserver observer);
    Task<double> GetTemperature();
}