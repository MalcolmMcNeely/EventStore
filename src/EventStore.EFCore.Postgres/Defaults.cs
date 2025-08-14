namespace EventStore.EFCore.Postgres;

public static class Defaults
{
    public static class Cursors
    {
        public const string AllStreamCursor = nameof(AllStreamCursor);
    }

    public static class Commands
    {
        public const string CommandPartition = "$command";
    }

    public static class Streams
    {
        public const string AllStreamPartition = "$All";
    }
}