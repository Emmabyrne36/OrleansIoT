using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration.Overrides;
using Orleans.Storage;
using OrleansIoT.FileStorage;

namespace OrleansIoT.TestSilo.Factories;

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