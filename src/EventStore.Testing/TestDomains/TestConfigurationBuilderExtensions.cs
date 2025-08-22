using EventStore.Commands;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.Idempotency;
using EventStore.Testing.TestDomains.MultiStreamProjection;
using EventStore.Testing.TestDomains.Simple;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Testing.TestDomains;

public static class TestConfigurationBuilderExtensions
{
    public static TestConfigurationBuilder WithSimpleDomain(this TestConfigurationBuilder builder, bool scoped = false)
    {
        if (scoped)
        {
            builder.HostBuilder.Services.AddScoped<ProjectionBuilder<SimpleProjection>, SimpleProjectionBuilder>();
            builder.HostBuilder.Services.AddScoped<IProjection, SimpleProjection>();
            builder.HostBuilder.Services.AddScoped<ICommandHandler<SimpleCommand>, SimpleCommandHandler>();
        }
        else
        {
            builder.HostBuilder.Services.AddTransient<ProjectionBuilder<SimpleProjection>, SimpleProjectionBuilder>();
            builder.HostBuilder.Services.AddTransient<IProjection, SimpleProjection>();
            builder.HostBuilder.Services.AddTransient<ICommandHandler<SimpleCommand>, SimpleCommandHandler>();
        }

        return builder;
    }

    public static TestConfigurationBuilder WithMultiStreamProjectionDomain(this TestConfigurationBuilder builder, bool scoped = false)
    {
        if (scoped)
        {
            builder.HostBuilder.Services.AddScoped<ProjectionBuilder<FirstKeyedProjection>, FirstKeyedProjectionBuilder>();
            builder.HostBuilder.Services.AddScoped<IProjection, FirstKeyedProjection>();
            builder.HostBuilder.Services.AddScoped<ProjectionBuilder<SecondKeyedProjection>, SecondKeyedProjectionBuilder>();
            builder.HostBuilder.Services.AddScoped<IProjection, SecondKeyedProjection>();
            builder.HostBuilder.Services.AddScoped<ICommandHandler<MultiStreamProjectionCommand>, MultiStreamProjectionCommandHandler>();
        }
        else
        {
            builder.HostBuilder.Services.AddTransient<ProjectionBuilder<FirstKeyedProjection>, FirstKeyedProjectionBuilder>();
            builder.HostBuilder.Services.AddTransient<IProjection, FirstKeyedProjection>();
            builder.HostBuilder.Services.AddTransient<ProjectionBuilder<SecondKeyedProjection>, SecondKeyedProjectionBuilder>();
            builder.HostBuilder.Services.AddTransient<IProjection, SecondKeyedProjection>();
            builder.HostBuilder.Services.AddTransient<ICommandHandler<MultiStreamProjectionCommand>, MultiStreamProjectionCommandHandler>();
        }

        return builder;
    }

    public static TestConfigurationBuilder WithIdempotencyDomain(this TestConfigurationBuilder builder, bool scoped = false)
    {
        if (scoped)
        {
            builder.HostBuilder.Services.AddScoped<ProjectionBuilder<IdempotencyProjection>, IdempotencyProjectionBuilder>();
            builder.HostBuilder.Services.AddScoped<IProjection, IdempotencyProjection>();
            builder.HostBuilder.Services.AddScoped<ICommandHandler<IdempotencyCommand>, IdempotencyCommandHandler>();
        }
        else
        {
            builder.HostBuilder.Services.AddTransient<ProjectionBuilder<IdempotencyProjection>, IdempotencyProjectionBuilder>();
            builder.HostBuilder.Services.AddTransient<IProjection, IdempotencyProjection>();
            builder.HostBuilder.Services.AddTransient<ICommandHandler<IdempotencyCommand>, IdempotencyCommandHandler>();
        }

        return builder;
    }
}