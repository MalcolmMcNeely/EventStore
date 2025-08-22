using System;
using EventStore.EFCore.Postgres.Events.Transport;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventStore.EFCore.Postgres.Tests.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RowKey = table.Column<int>(type: "integer", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CommandType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CausationId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => new { x.Key, x.RowKey });
                });

            migrationBuilder.CreateTable(
                name: "EventCursorEntities",
                columns: table => new
                {
                    CursorName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenEvent = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCursorEntities", x => x.CursorName);
                });

            migrationBuilder.CreateTable(
                name: "EventStreams",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RowKey = table.Column<int>(type: "integer", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CausationId = table.Column<string>(type: "text", nullable: false),
                    Envelope = table.Column<Envelope>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStreams", x => new { x.Key, x.RowKey });
                });

            migrationBuilder.CreateTable(
                name: "FirstKeyedProjections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstKeyedProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyAggregateRoots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<bool>(type: "boolean", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyAggregateRoots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyProjections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MultiStreamProjectionAggregateRoots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiStreamProjectionAggregateRoots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueuedEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Envelope = table.Column<Envelope>(type: "jsonb", nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecondKeyedProjections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondKeyedProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleAggregateRoots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleAggregateRoots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleProjections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleProjections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commands_Key_RowKey",
                table: "Commands",
                columns: new[] { "Key", "RowKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventStreams_Key_RowKey",
                table: "EventStreams",
                columns: new[] { "Key", "RowKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "EventCursorEntities");

            migrationBuilder.DropTable(
                name: "EventStreams");

            migrationBuilder.DropTable(
                name: "FirstKeyedProjections");

            migrationBuilder.DropTable(
                name: "IdempotencyAggregateRoots");

            migrationBuilder.DropTable(
                name: "IdempotencyProjections");

            migrationBuilder.DropTable(
                name: "MultiStreamProjectionAggregateRoots");

            migrationBuilder.DropTable(
                name: "QueuedEvents");

            migrationBuilder.DropTable(
                name: "SecondKeyedProjections");

            migrationBuilder.DropTable(
                name: "SimpleAggregateRoots");

            migrationBuilder.DropTable(
                name: "SimpleProjections");
        }
    }
}
