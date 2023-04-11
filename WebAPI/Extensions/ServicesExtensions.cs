using Orleans;
using WebAPI.HostedServices;

namespace WebAPI.Extensions;

public static class ServicesExtensions
{
    public static void AddOrleansClient(this IServiceCollection services)
    {
        services.AddSingleton<OrleansClientHostedService>();
        services.AddSingleton<IHostedService>(sp => sp.GetService<OrleansClientHostedService>());
        services.AddSingleton<IClusterClient>(sp => sp.GetService<OrleansClientHostedService>().Client);
        services.AddSingleton<IGrainFactory>(sp => sp.GetService<OrleansClientHostedService>().Client);
    }
}