using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Testing.Configuration;

public static class TestConfiguration
{
    public static string DatabaseConnectionString { set; get; } = string.Empty;
    public static string AzuriteConnectionString { set; get; } = string.Empty;

    static TestConfigurationBuilder? _testConfigurationBuilder;

    public static TestConfigurationBuilder Configure()
    {
        _testConfigurationBuilder = new(DatabaseConnectionString, AzuriteConnectionString);
        return _testConfigurationBuilder;
    }

    public static T Resolve<T>() where T : class
    {
        if (_testConfigurationBuilder?.ServiceHost is null)
        {
            throw new TestConfigurationException("TestConfiguration has not been configured");
        }

        return _testConfigurationBuilder.ServiceHost.Services.GetRequiredService<T>();
    }

    public static T ResolveScoped<T>() where T : class
    {
        if (_testConfigurationBuilder?.ServiceHost is null)
        {
            throw new TestConfigurationException("TestConfiguration has not been configured");
        }

        var scope = _testConfigurationBuilder.ServiceHost.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}