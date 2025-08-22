using EventStore.Testing.Configuration;
using Testcontainers.PostgreSql;

namespace EventStore.EFCore.Postgres.Tests;

[SetUpFixture]
public class PostgresTestFixture
{
    readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("EventStore")
        .WithUsername("postgres")
        .WithPassword("password")
        .WithImage("postgres:latest")
        .Build();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _dbContainer.StartAsync();
        TestConfiguration.DatabaseConnectionString = _dbContainer.GetConnectionString();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}