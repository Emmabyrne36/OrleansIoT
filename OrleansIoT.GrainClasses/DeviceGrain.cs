using Orleans;
using Orleans.Runtime;
using OrleansIoT.Core.Constants;
using OrleansIoT.GrainClasses.States;
using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.GrainClasses;

public class DeviceGrain : Grain, IDeviceGrain
{
    private readonly IPersistentState<DeviceGrainState> _profile;

    public DeviceGrain(
        [PersistentState(nameof(DeviceGrain), OrleansIoTProviders.FileStorageProvider)] IPersistentState<DeviceGrainState> profile)
    {
        _profile = profile;
    }

    public override Task OnActivateAsync()
    {
        var id = this.GetPrimaryKeyLong();
        Console.WriteLine($"Activated {id}");
        Console.WriteLine($"Activated state = {_profile.State.LastValue}");

        return base.OnActivateAsync();
    }

    public async Task SetTemperature(double value)
    {
        if (_profile.State.LastValue < 100 && value >= 100)
        {
            Console.WriteLine($"High temperature recorded: {value}");
        }

        if (_profile.State.LastValue != value)
        {
            _profile.State.LastValue = value;
            await _profile.WriteStateAsync();
        }
    }
}


//[StorageProvider(ProviderName = OrleansIoTConstants.FileStorageProvider)]
//public class DeviceGrain : Grain<DeviceGrainState>, IDeviceGrain
//{
//    public override Task OnActivateAsync()
//    {
//        var id = this.GetPrimaryKeyLong();
//        Console.WriteLine($"Activated {id}");
//        Console.WriteLine($"Activated state = {State.LastValue}");

//        return base.OnActivateAsync();
//    }

//    public async Task SetTemperature(double value)
//    {
//        if (State.LastValue < 100 && value >= 100)
//        {
//            Console.WriteLine($"High temperature recorded: {value}");
//        }

//        if (State.LastValue != value)
//        {
//            State.LastValue = value;
//            await WriteStateAsync();
//        }
//    }
//}