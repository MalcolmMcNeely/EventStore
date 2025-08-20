using EventStore.Events.Streams;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.Simple;
using EventStore.Testing.Utility;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Events.Streams;

public abstract class EventStreamTest : PostgresIntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(EventStreamTest).Assembly)
            .Build();
    }
}

public class WhenPublishingAnEvent : EventStreamTest
{
    IEventStreamFactory _eventStreamFactory;
    const string StreamName = "oneStream";

    [SetUp]
    public async Task Setup()
    {
        _eventStreamFactory = GetService<IEventStreamFactory>();
        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(StreamName).PublishAsync(new SimpleEvent()), 1);
    }

    [Test]
    public async Task it_can_count_one_event()
    {
        var numberOfEvents = await _eventStreamFactory.For(StreamName).GetCountAsync();
        await Verify(numberOfEvents);
    }

    [Test]
    public async Task it_can_get_the_event()
    {
        var events = await _eventStreamFactory.For(StreamName).GetAllEventsAsync().ToListAsync();
        await Verify(events);
    }
}

public class WhenPublishingManyEvents : EventStreamTest
{
    IEventStreamFactory _eventStreamFactory;
    const string StreamName = "anotherStream";

    [SetUp]
    public async Task Setup()
    {
        _eventStreamFactory = GetService<IEventStreamFactory>();
        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(StreamName).PublishAsync(new SimpleEvent()), 2000);
    }


    [Test]
    public async Task it_can_count_all_events()
    {
        var numberOfEvents = await _eventStreamFactory.For(StreamName).GetCountAsync();
        await Verify(numberOfEvents);
    }

    [Test]
    public async Task it_can_get_all_events()
    {
        var events = await _eventStreamFactory.For(StreamName).GetAllEventsAsync().ToListAsync();
        await Verify(events);
    }
}