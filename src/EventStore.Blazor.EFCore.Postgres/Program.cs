using EventStore;
using MudBlazor.Services;
using EventStore.Blazor.EFCore.Postgres.Components;
using EventStore.Blazor.EFCore.Postgres.Services.Commands;
using EventStore.Blazor.EFCore.Postgres.Services.Events;
using EventStore.Commands;
using EventStore.EFCore.Postgres;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain;
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

builder.Services.AddHostedService<ChangeColourBackgroundService>();

builder.AddCoreServices();
builder.AddEventBroadcaster();
builder.AddEventPump();
builder.AddPostgresServices(databaseConnectionString, typeof(AppDomainNamespace).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();