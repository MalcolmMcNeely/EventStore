namespace EventStore.Blazor.EFCore.Postgres.Extensions;

public static class HealthCheckConfigurationExtensions
{
    public static WebApplicationBuilder AddHealthChecksConfiguration(this WebApplicationBuilder builder, string databaseConnectionString)
    {
        builder.Services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString);

        return builder;
    }
}