using System.Threading.Channels;

namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public static class BackgroundServiceHostBuilderExtensions
{
    public static void AddBackgroundServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddHostedService<ChangeColourBackgroundService>();
        hostBuilder.Services.AddSingleton<Channel<ChangeColourBackgroundServiceRequest>>(_ =>
            Channel.CreateUnbounded<ChangeColourBackgroundServiceRequest>(new UnboundedChannelOptions
            {
                SingleReader = true,
            }));
        hostBuilder.Services.AddScoped<IChangeColourChannelService, ChangeColourChannelService>();
    }
}