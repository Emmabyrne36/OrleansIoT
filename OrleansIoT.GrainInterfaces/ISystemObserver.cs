using Orleans;

namespace OrleansIoT.GrainInterfaces;

public interface ISystemObserver : IGrainObserver
{
    void HighTemperature(double value);
}