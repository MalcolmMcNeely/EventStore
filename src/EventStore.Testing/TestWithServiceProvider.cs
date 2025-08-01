namespace EventStore.Testing;

public abstract class TestWithServiceProvider
{
    protected IServiceProvider ServiceProvider { get; set; }
}