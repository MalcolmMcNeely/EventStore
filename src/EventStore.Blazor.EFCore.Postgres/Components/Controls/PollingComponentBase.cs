using Microsoft.AspNetCore.Components;

namespace EventStore.Blazor.EFCore.Postgres.Components.BaseControls;

public abstract class PollingComponentBase : ComponentBase, IDisposable
{
    protected DateTime LastPoll = DateTime.MinValue;

    readonly CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Task.Run(() => StartPollingAsync(_cancellationTokenSource.Token));
        }
    }

    protected abstract Task StartPollingAsync(CancellationToken token);

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}