using System.Threading.Channels;

namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public class ChangeColourChannelService(Channel<ChangeColourBackgroundServiceRequest> channel) : IChangeColourChannelService
{
    public async Task PauseBackgroundServiceAsync(CancellationToken token = default)
    {
        await channel.Writer.WriteAsync(new ChangeColourBackgroundServiceRequest(false, null), token);
    }

    public async Task ResumeBackgroundServiceAsync(CancellationToken token = default)
    {
        await channel.Writer.WriteAsync(new ChangeColourBackgroundServiceRequest(true, null), token);
    }

    public async Task ToggleBackgroundServiceAsync(CancellationToken token = default)
    {
        await channel.Writer.WriteAsync(new ChangeColourBackgroundServiceRequest(null, true), token);
    }
}

