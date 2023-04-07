using Orleans.Concurrency;

namespace OrleansIoT.Core.GrainObjects;

[Immutable]
public class TemperatureReading
{
    public TemperatureReading(long deviceId, DateTime time, double value)
    {
        DeviceId = deviceId;
        Time = time;
        Value = value;
    }

    public long DeviceId { get; set; }
    public DateTime Time { get; set; }
    public double Value { get; set; }
}