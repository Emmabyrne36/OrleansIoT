using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansIoT.Core.Constants;
using OrleansIoT.TestSilo.Extensions;
using System.Net;

try
{
    var host = await BuildAndStartSiloAsync();

    Console.WriteLine("Press Enter to terminate...");
    Console.ReadLine();

    await host.StopAsync();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task<IHost> BuildAndStartSiloAsync()
{
    var host = new HostBuilder()
        .UseOrleans(builder =>
        {
            builder.UseLocalhostClustering()
                .AddFileGrainStorage(OrleansIoTStorageProviders.FileStorageProvider, opts =>
                {
                    opts.RootDirectory = "C:\\data\\orleansIoT";
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansIoT";
                })
                .Configure<EndpointOptions>(
                    options => options.AdvertisedIPAddress = IPAddress.Loopback)
                //.AddMemoryGrainStorage(OrleansIoTConstants.OrleansMemoryProvider)
                .ConfigureLogging(logging => logging.AddConsole());
        })
        .Build();

    await host.StartAsync();

    return host;
}