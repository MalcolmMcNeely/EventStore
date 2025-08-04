using System.Text;
using EventStore.Commands;
using EventStore.Commands.Dispatching;
using EventStore.EFCore.Postgres.Database;
using EventStore.Events;
using EventStore.Events.Transport;
using EventStore.Testing.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EventStore.Testing;

public abstract class IntegrationTest
{
    ICommandDispatcher _commandDispatcher;
    IEventBroadcaster _eventBroadcaster;
    IEventPump _eventPump;
    IEventTransport _eventTransport;

    [OneTimeSetUp]
    public void DefaultConfiguration()
    {
        TestConfiguration
            .Configure()
            .WithInMemoryServices()
            .Build();
    }

    [SetUp]
    public async Task DefaultSetup()
    {
        _commandDispatcher = GetService<ICommandDispatcher>()!;
        _eventBroadcaster = GetService<IEventBroadcaster>()!;
        _eventPump = GetService<IEventPump>()!;
        _eventTransport = GetService<IEventTransport>()!;

        if (TestConfiguration.IsEFCoreTest)
        {
            await DeleteAllRowsFromAllTablesAsync();
        }
    }

    protected T? GetService<T>() where T : class => TestConfiguration.Resolve<T>();

    protected async Task DispatchCommandAsync(ICommand command) => await _commandDispatcher.DispatchAsync(command);

    protected async Task SendEventAsync(IEvent @event)
    {
        await _eventTransport.WriteEventAsync(@event);
        await _eventPump.PublishEventsAsync();
        await _eventBroadcaster.BroadcastEventAsync();
    }

    async Task DeleteAllRowsFromAllTablesAsync()
    {
        await using var connection = new NpgsqlConnection(TestConfiguration.DatabaseConnectionString);
        await connection.OpenAsync();

        var tableNames = new List<string>();

        await using var queryTables = new NpgsqlCommand("SELECT tablename FROM pg_tables WHERE schemaname = 'public';", connection);
        await using (var reader = await queryTables.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                tableNames.Add(reader.GetString(0));
            }
        }

        if (tableNames.Count == 0)
        {
            return;
        }

        var truncateString = new StringBuilder("TRUNCATE ");
        truncateString.AppendJoin(", ", tableNames.Select(name => $"\"{name}\""));
        truncateString.Append(" RESTART IDENTITY CASCADE;");

        var truncateCommand = new NpgsqlCommand(truncateString.ToString(), connection);
        await truncateCommand.ExecuteNonQueryAsync();
    }
}