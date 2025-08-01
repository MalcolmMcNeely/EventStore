using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Testing.Configuration;

public static class TestConfiguration
{
    public static bool IsConfigured => _testConfigurationBuilder?.ServiceHost is not null;

    static TestConfigurationBuilder? _testConfigurationBuilder;

    public static TestConfigurationBuilder Configure()
    {
        _testConfigurationBuilder = new();
        return _testConfigurationBuilder;
    }

    public static T Resolve<T>() where T : class
    {
        if (IsConfigured is false)
        {
            throw new TestConfigurationException("TestConfiguration has not been configured");
        }
        
        return _testConfigurationBuilder!.ServiceHost!.Services.GetService<T>()!;
    }
}