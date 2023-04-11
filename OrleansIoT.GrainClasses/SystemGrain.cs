using Orleans;
using OrleansIoT.Core.Factories;
using OrleansIoT.Core.GrainObjects;
using OrleansIoT.Core.Observers;
using OrleansIoT.GrainInterfaces;


namespace OrleansIoT.GrainClasses;

public class SystemGrain : Grain, ISystemGrain
{
    private Dictionary<long, double> _temperatures;
    private ObserverManager<ISystemObserver> _observers;

    public override Task OnActivateAsync()
    {
        _temperatures = new Dictionary<long, double>();
        _observers = new ObserverManager<ISystemObserver>(
            TimeSpan.FromMinutes(5), 
            OrleansLoggerFactory.CreateLogger<SystemGrain>(), 
            nameof(SystemGrain));

        RegisterTimer(Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

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

        return Task.CompletedTask;
    }

    public Task Subscribe(ISystemObserver observer)
    {
        _observers.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public Task Unsubscribe(ISystemObserver observer)
    {
        _observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public Task<double> GetTemperature()
    {
        if (_temperatures.Count == 0)
        {
            return (Task<double>)Task.CompletedTask;
        }

        return Task.FromResult(_temperatures.Values.Average());
    }

    private Task Callback(object callbackState)
    {
        if (_temperatures.Count == 0)
        {
            return Task.CompletedTask;
        }

        var value = _temperatures.Values.Average();
        if (value > 100)
        {
            _observers.Notify(x => x.HighTemperature(value));
        }

        return Task.CompletedTask;
    }
}