using EventStore.Events.Streams;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.Simple;
using EventStore.Testing.Utility;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Events.Streams;

public class EventStreamTests : IntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(EventStreamTests).Assembly)
            .Build();
    }

    [Test]
    public async Task it_can_publish_an_event()
    {
        var streamName = "oneStream";
        var eventStreamFactory = GetService<IEventStreamFactory>();
        
        await TestUtility.InvokeManyAsync(async () => await eventStreamFactory.For(streamName).PublishAsync(new SimpleEvent()), 1);

        var numberOfEvents = await eventStreamFactory.For(streamName).GetCountAsync();
        var events = await eventStreamFactory.For(streamName).GetAllEventsAsync().ToListAsync();

        await Verify(new { NumberOfEvents = numberOfEvents, Events = events });
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var streamName = "anotherStream";
        var eventStreamFactory = GetService<IEventStreamFactory>();

        await TestUtility.InvokeManyAsync(async () => await eventStreamFactory.For(streamName).PublishAsync(new SimpleEvent()), 2000);

        var numberOfEvents = await eventStreamFactory.For(streamName).GetCountAsync();
        var events = await eventStreamFactory.For(streamName).GetAllEventsAsync().ToListAsync();

        await Verify(new { NumberOfEvents = numberOfEvents, Events = events });
    }
}
