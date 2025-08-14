using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EventStore.EFCore.Postgres.Database;

public class SlowQueryInterceptor : DbCommandInterceptor
{
    const int _slowQueryThreshold = 1000;

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > _slowQueryThreshold)
        {
            Console.WriteLine($"Slow query ({eventData.Duration.TotalMilliseconds}ms) executed in {command.CommandText}");
        }

        return base.ReaderExecuted(command, eventData, result);
    }
}