using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using OrleansIoT.FileStorage;
using OrleansIoT.TestSilo.Factories;

namespace OrleansIoT.TestSilo.Extensions;

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