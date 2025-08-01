using EventStore.Testing.Configuration;

namespace EventStore.Testing;

public abstract class IntegrationTest
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        TestConfiguration
            .Configure()
            .WithInMemoryServices()
            .Build();
    }

    protected T GetService<T>() where T : class
    {
        return TestConfiguration.Resolve<T>();
    }
}
