using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansIoT.GrainInterfaces;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly IClusterClient _client;

    public SystemController(IClusterClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpGet]
    public async Task<ActionResult<double>> Get(string id)
    {
        //var systemGrain = GrainFactory.GetGrain<ISystemGrain>(0, _profile.State.System ?? OrleansIoTConstants.DefaultDevice);
        
        var systemGrain = _client.GetGrain<ISystemGrain>(0, id);

        if (systemGrain is null)
        {
            return NotFound($"The grain with Id {id} was not found.");
        }

        return Ok(await systemGrain.GetTemperature());
    }
}