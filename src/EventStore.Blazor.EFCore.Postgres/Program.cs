using EventStore;
using EventStore.Blazor.EFCore.Postgres.BackgroundServices;
using MudBlazor.Services;
using EventStore.Blazor.EFCore.Postgres.Components;
using EventStore.Blazor.EFCore.Postgres.Services.Commands;
using EventStore.Blazor.EFCore.Postgres.Services.Events;
using EventStore.Commands;
using EventStore.EFCore.Postgres;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain;
using EventStore.SampleApp.Domain.Accounts.Commands;
using EventStore.SampleApp.Domain.Accounts.Projections;
using EventStore.SampleApp.Domain.TrafficLights.Commands;
using EventStore.SampleApp.Domain.TrafficLights.Projections;

var builder = WebApplication.CreateBuilder(args);
var databaseConnectionString = builder.Configuration["ConnectionStrings:Postgres"]!;

builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICommandService, CommandService>();

builder.Services.AddScoped<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
builder.Services.AddScoped<IProjection, TrafficLightProjection>();
builder.Services.AddScoped<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

// TODO register better
builder.Services.AddScoped<ProjectionBuilder<IndividualAccountProjection>, IndividualAccountProjectionBuilder>();
builder.Services.AddScoped<IProjection, IndividualAccountProjection>();
builder.Services.AddScoped<ProjectionBuilder<TotalBusinessAccountProjection>, TotalBusinessAccountProjectionBuilder>();
builder.Services.AddScoped<IProjection, TotalBusinessAccountProjection>();

builder.Services.AddScoped<ICommandHandler<OpenAccount>, AccountsCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CloseAccount>, AccountsCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreditAccount>, AccountsCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DebitAccount>, AccountsCommandHandler>();

builder.AddBackgroundServices();

builder.AddCoreServices();
builder.AddEventBroadcaster();
builder.AddEventPump();
builder.AddPostgresServices(x =>
{
    x.ConnectionString = databaseConnectionString;
    x.MigrationsAssembly = typeof(DbContextFactory).Assembly;
    x.AggregateAssemblies = [typeof(AppDomainNamespace).Assembly];
    x.AutoMigrate = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();