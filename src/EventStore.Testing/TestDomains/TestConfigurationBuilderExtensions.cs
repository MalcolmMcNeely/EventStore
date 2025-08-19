using EventStore.Commands;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.SimpleTestDomain;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Testing.TestDomains;

public static class TestConfigurationBuilderExtensions
{
    public static TestConfigurationBuilder WithSimpleTestDomain(this TestConfigurationBuilder builder, bool scoped = false)
    {
        if (scoped)
        {
            builder.HostBuilder.Services.AddScoped<ProjectionBuilder<TestProjection>, TestDefaultKeyProjectionBuilder>();
            builder.HostBuilder.Services.AddScoped<IProjection, TestProjection>();
            builder.HostBuilder.Services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
        }
        else
        {
            builder.HostBuilder.Services.AddTransient<ProjectionBuilder<TestProjection>, TestDefaultKeyProjectionBuilder>();
            builder.HostBuilder.Services.AddTransient<IProjection, TestProjection>();
            builder.HostBuilder.Services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
        }

        return builder;
    }
}