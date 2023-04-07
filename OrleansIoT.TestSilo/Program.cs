using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Configuration.Overrides;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using OrleansIoT.Core;
using OrleansIoT.FileStorage;
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
                .AddFileGrainStorage(OrleansIoTConstants.FileStorageProvider, opts =>
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


public static class FileSiloBuilderExtensions
{
    public static ISiloBuilder AddFileGrainStorage(
        this ISiloBuilder builder,
        string providerName,
        Action<FileGrainStorageOptions> options)
    {
        return builder.ConfigureServices(
            services => services.AddFileGrainStorage(providerName, options));
    }

    public static IServiceCollection AddFileGrainStorage(
        this IServiceCollection services,
        string providerName,
        Action<FileGrainStorageOptions> options)
    {
        services.AddOptions<FileGrainStorageOptions>(providerName).Configure(options);

        return services.AddSingletonNamedService(providerName, FileStorageProviderFactory.Create)
            .AddSingletonNamedService(
                providerName,
                (s, n) => s.GetRequiredServiceByName<IGrainStorage>(n));
    }
}

public static class FileStorageProviderFactory
{
    public static IGrainStorage Create(
        IServiceProvider services, string name)
    {
        var optionsSnapshot =
            services.GetRequiredService<IOptionsSnapshot<FileGrainStorageOptions>>();

        return ActivatorUtilities.CreateInstance<FileStorageProvider>(
            services,
            name,
            optionsSnapshot.Get(name),
            services.GetProviderClusterOptions(name));
    }
}