using OrleansIoT.GrainInterfaces;

namespace OrleansIoT.GrainClasses.States;

public class DeviceGrainState : IDeviceGrainState
{
    public object State { get; set; }
    public Type Type { get; }
    public string ETag { get; set; }
    public bool RecordExists { get; set; }
    public double LastValue { get; set; }
}