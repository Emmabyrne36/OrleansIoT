using Orleans;

namespace OrleansIoT.GrainInterfaces;

public interface IDecodeGrain : IGrainWithIntegerKey
{
    Task Decode(string message);
}