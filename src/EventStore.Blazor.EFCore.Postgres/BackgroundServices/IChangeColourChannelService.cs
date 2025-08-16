namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public interface IChangeColourChannelService
{
    Task PauseBackgroundServiceAsync(CancellationToken token = default);
    Task ResumeBackgroundServiceAsync(CancellationToken token = default);
    Task ToggleBackgroundServiceAsync(CancellationToken token = default);
}