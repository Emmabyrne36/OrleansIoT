﻿using Orleans;

namespace OrleansIoT.GrainInterfaces;

public interface IDeviceGrain : IGrainWithIntegerKey
{
    Task SetTemperature(double value);
    Task JoinSystem(string name);
    Task<double> GetTemperature();
}