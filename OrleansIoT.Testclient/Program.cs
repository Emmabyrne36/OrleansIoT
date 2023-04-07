using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using OrleansIoT.Core.Constants;
using OrleansIoT.GrainInterfaces;

try
{
    await using var client = await ConnectClientAsync();
    await DoClientWorkAsync(client);

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"\nException while trying to run grainFactory: {ex.Message}");
    Console.WriteLine("Make sure the silo the grainFactory is trying to connect to is running.");
    Console.WriteLine("\nPress any key to exit.");
    Console.ReadKey();
    return 1;
}

static async Task<IClusterClient> ConnectClientAsync()
{
    var client = new ClientBuilder()
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansIoT";
        })
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await client.Connect();
    Console.WriteLine("Client successfully connected to silo host \n");

    return client;
}

static async Task DoClientWorkAsync(IGrainFactory grainFactory)
{
    AddDevices(grainFactory);
    var grain = grainFactory.GetGrain<IDecodeGrain>(0);

    while (true)
    {
        await grain.Decode(Console.ReadLine());
    }
}

static void AddDevices(IGrainFactory grainFactory)
{
    var system = OrleansIoTConstants.DefaultDevice;

    JoinGrainToSystem(grainFactory, system, 3);
    JoinGrainToSystem(grainFactory, system, 4);
    JoinGrainToSystem(grainFactory, system, 5);
}

static void JoinGrainToSystem(IGrainFactory grainFactory, string system, long id)
{
    var deviceGrain = grainFactory.GetGrain<IDeviceGrain>(id);
    deviceGrain.JoinSystem(system).Wait();
}

//static async Task DoClientWorkAsync(IClusterClient client)
//{
//    var grain = client.GetGrain<IDeviceGrain>(0);

//    while (true)
//    {
//        var isValid = double.TryParse(Console.ReadLine(), out var newTemperature);
//        if (isValid)
//        {
//            await grain.SetTemperature(newTemperature);
//        }
//        else
//        {
//            Console.WriteLine("The input is not in the correct format. Please enter a valid number.");
//        }
//    }
//}