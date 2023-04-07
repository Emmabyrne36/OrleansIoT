using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using OrleansIoT.GrainInterfaces;

try
{
    await using var client = await ConnectClientAsync();
    await DoClientWorkAsync(client);

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"\nException while trying to run client: {ex.Message}");
    Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
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

static async Task DoClientWorkAsync(IClusterClient client)
{
    var grain = client.GetGrain<IDeviceGrain>(0);

    while (true)
    {
        await grain.SetTemperature(double.Parse(Console.ReadLine()));
    }
}