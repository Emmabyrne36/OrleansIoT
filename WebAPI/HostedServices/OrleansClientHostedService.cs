using Orleans;
using Orleans.Configuration;

namespace WebAPI.HostedServices;

public class OrleansClientHostedService : IHostedService
{
    public IClusterClient Client { get; }

    public OrleansClientHostedService(ILoggerProvider loggerProvider)
    {
        Client = new ClientBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "OrleansIoT";
            })
            .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Client.Connect();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.Close();
        Client.Dispose();
    }
}