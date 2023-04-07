using Orleans;
using OrleansIoT.Core.GrainObjects;

namespace OrleansIoT.GrainInterfaces;

public interface ISystemGrain : IGrainWithIntegerCompoundKey
{
    Task SetTemperature(TemperatureReading temperatureReading);
}