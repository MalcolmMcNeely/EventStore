using System;
using EventStore.EFCore.Postgres.Events.Transport;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventStore.SampleApp.EFCore.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventCursorEntities",
                columns: table => new
                {
                    CursorName = table.Column<string>(type: "text", nullable: false),
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
                    Key = table.Column<string>(type: "text", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Envelope = table.Column<Envelope>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStreams", x => new { x.Key, x.RowKey });
                });

            migrationBuilder.CreateTable(
                name: "QueuedEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Envelope = table.Column<Envelope>(type: "jsonb", nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrafficLightProjections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false),
                    Colour = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficLightProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrafficLights",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficLights", x => x.Id);
                });

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
                name: "EventCursorEntities");

            migrationBuilder.DropTable(
                name: "EventStreams");

            migrationBuilder.DropTable(
                name: "QueuedEvents");

            migrationBuilder.DropTable(
                name: "TrafficLightProjections");

            migrationBuilder.DropTable(
                name: "TrafficLights");
        }
    }
}
