using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansIoT.GrainInterfaces;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeviceController : ControllerBase
{
    private readonly IClusterClient _client;

    public DeviceController(IClusterClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpGet]
    public async Task<ActionResult<double>> Get(long id)
    {
        var deviceGrain = _client.GetGrain<IDeviceGrain>(id);

        if (deviceGrain is null)
        {
            return NotFound($"The grain with Id {id} was not found.");
        }

        return Ok(await deviceGrain.GetTemperature());
    }

    [HttpPost]
    public ActionResult Post([FromBody] string message)
    {
        var decodeGrain = _client.GetGrain<IDecodeGrain>(0);

        if (decodeGrain is null)
        {
            return NotFound($"The decode grain was not found.");
        }

        return Ok(decodeGrain.Decode(message));
    }
}