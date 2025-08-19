using EventStore.Events.Streams;
using EventStore.Testing.Configuration;
using EventStore.Testing.SimpleTestDomain;
using EventStore.Testing.Utility;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Events.Streams;

public class EventStreamTests : PostgresIntegrationTest
{
    IEventStreamFactory _eventStreamFactory;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(EventStreamTests).Assembly)
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        _eventStreamFactory = GetService<IEventStreamFactory>();
    }

    [Test]
    public async Task it_can_publish_an_event()
    {
        var streamName = "oneStream";

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new TestEvent()), 1);

        var numberOfEvents = await _eventStreamFactory.For(streamName).GetCountAsync();
        var events = await _eventStreamFactory.For(streamName).GetAllEventsAsync().ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(numberOfEvents, Is.EqualTo(1));
            Assert.That(events, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var streamName = "anotherStream";

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new TestEvent()), 2000);

        var numberOfEvents = await _eventStreamFactory.For(streamName).GetCountAsync();
        var events = await _eventStreamFactory.For(streamName).GetAllEventsAsync().ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(numberOfEvents, Is.EqualTo(2000));
            Assert.That(events, Has.Count.EqualTo(2000));
        });
    }
}