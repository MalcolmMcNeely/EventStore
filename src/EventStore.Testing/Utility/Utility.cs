namespace EventStore.Testing.Utility;

public static class TestUtility
{
    public static async Task InvokeManyAsync(Func<Task> async, int times)
    {
        var tasks = Enumerable.Range(0, times).Select(_ => async.Invoke());
        await Task.WhenAll(tasks);
    }
}