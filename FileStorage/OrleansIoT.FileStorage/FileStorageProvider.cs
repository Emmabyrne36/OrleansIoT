using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Storage;
using OrleansIoT.GrainClasses.States;
using System.Globalization;

namespace OrleansIoT.FileStorage;

public class FileStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
{
    //https://learn.microsoft.com/en-us/dotnet/orleans/tutorials-and-samples/custom-grain-storage?pivots=orleans-3-x

    public string Name { get; set; }
    private readonly FileGrainStorageOptions _options;
    private readonly ClusterOptions _clusterOptions;

    public FileStorageProvider(
        string storageName,
        FileGrainStorageOptions options,
        IOptions<ClusterOptions> clusterOptions)
    {
        Name = storageName;
        _options = options;
        _clusterOptions = clusterOptions.Value;
    }

    public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
        var fileInfo = GetFileInfo(grainType, grainReference);

        if (!fileInfo.Exists)
        {
            grainState.State = Activator.CreateInstance(grainState.State.GetType());
            return;
        }

        using var stream = fileInfo.OpenText();
        var json = await stream.ReadToEndAsync();
        grainState.State = JsonConvert.DeserializeObject<DeviceGrainState>(json);

        grainState.ETag = fileInfo.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture);
    }

    public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
        var json = JsonConvert.SerializeObject(grainState.State);

        var fileInfo = GetFileInfo(grainType, grainReference);
        await using var writer = new StreamWriter(fileInfo.Open(FileMode.Create, FileAccess.Write));

        await writer.WriteAsync(json);
        fileInfo.Refresh();
        grainState.ETag = fileInfo.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture);
    }

    public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
        var fileInfo = GetFileInfo(grainType, grainReference);
        grainState.ETag = null;
        grainState.State = Activator.CreateInstance(grainState.State.GetType());
        fileInfo.Delete();
        return Task.CompletedTask;
    }

    public Task Init(CancellationToken ct)
    {
        var directory = new DirectoryInfo(_options.RootDirectory);
        if (!directory.Exists)
        {
            directory.Create();
        }

        return Task.CompletedTask;
    }

    public void Participate(ISiloLifecycle lifecycle)
    {
        lifecycle.Subscribe(
            OptionFormattingUtilities.Name<FileStorageProvider>(Name),
            ServiceLifecycleStage.ApplicationServices,
            Init);
    }

    private FileInfo GetFileInfo(string grainType, GrainReference grainReference)
    {
        var fileName = GetKeyString(grainType, grainReference);
        return new FileInfo(Path.Combine(_options.RootDirectory, fileName));
    }

    private string GetKeyString(string grainType, GrainReference grainReference)
    {
        return $"{_clusterOptions.ServiceId}.{grainReference.ToKeyString()}.{grainType}";
    }
}