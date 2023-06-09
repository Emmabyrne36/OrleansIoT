﻿using Orleans;

namespace OrleansIoT.GrainInterfaces;

public interface  IDeviceGrainState : IGrainState
{
    public double LastValue { get; set; }
    public string System { get; set; }
}