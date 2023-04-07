using Orleans;
using Orleans.Concurrency;
using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.GrainClasses;

[StatelessWorker]
[Reentrant]
public class DecodeGrain : Grain, IDecodeGrain
{
    public Task Decode(string message)
    {
        var parts = message.Split(',');
        var grain = GrainFactory.GetGrain<IDeviceGrain>(long.Parse(parts[0]));
        return grain.SetTemperature(double.Parse(parts[1]));
    }
}