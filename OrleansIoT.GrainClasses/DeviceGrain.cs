using Orleans;
using Orleans.Providers;
using OrleansIoT.GrainInterfaces;
using OrleansIoT.Core;
using Orleans.Runtime;

namespace OrleansIoT.GrainClasses;

[StorageProvider(ProviderName = OrleansIoTConstants.FileStorageProvider)]
public class DeviceGrain : Grain<DeviceGrainState>, IDeviceGrain
{
    public override Task OnActivateAsync()
    {
        var id = this.GetPrimaryKeyLong();
        Console.WriteLine($"Activated {id}");
        Console.WriteLine($"Activated state = {State.LastValue}");

        return base.OnActivateAsync();
    }

    public async Task SetTemperature(double value)
    {
        if (State.LastValue < 100 && value >= 100)
        {
            Console.WriteLine($"High temperature recorded: {value}");
        }

        if (State.LastValue != value)
        {
            State.LastValue = value;
            await WriteStateAsync();
        }
    }
}

public class DeviceGrainState : IDeviceGrainState
{
    public object State { get; set; }
    public Type Type { get; }
    public string ETag { get; set; }
    public bool RecordExists { get; set; }
    public double LastValue { get; set; }
}



//public class DeviceGrain : Grain, IDeviceGrain
//{
//    private readonly IPersistentState<IDeviceGrainState> _profile;

//    public DeviceGrain(
//        [PersistentState("file", OrleansIoTConstants.FileStorageProvider)] IPersistentState<IDeviceGrainState> profile)
//    {
//        _profile = profile;
//    }

//    public override Task OnActivateAsync()
//    {
//        var id = this.GetPrimaryKeyLong();
//        Console.WriteLine($"Activated {id}");
//        Console.WriteLine($"Activated state = {_profile.State.LastValue}");

//        return base.OnActivateAsync();
//    }

//    public async Task SetTemperature(double value)
//    {
//        if (_profile.State.LastValue < 100 && value >= 100)
//        {
//            Console.WriteLine($"High temperature recorded: {value}");
//        }

//        if (_profile.State.LastValue != value)
//        {
//            _profile.State.LastValue = value;
//            await _profile.WriteStateAsync();
//        }
//    }
//}