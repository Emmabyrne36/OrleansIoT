using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using OrleansIoT.TestSilo.Extensions;
using OrleansIoT.Core.Constants;


using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;

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
                .AddFileGrainStorage(OrleansIoTProviders.FileStorageProvider, opts =>
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