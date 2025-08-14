using EventStore;
using EventStore.Azure;
using MudBlazor.Services;
using EventStore.Blazor.EFCore.Postgres.Components;
using EventStore.Commands;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain;
using EventStore.SampleApp.Domain.TrafficLights.Commands;
using EventStore.SampleApp.Domain.TrafficLights.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<ProjectionBuilder<TrafficLightProjection>, TrafficLightProjectionBuilder>();
builder.Services.AddTransient<IProjection, TrafficLightProjection>();

builder.Services.AddTransient<ICommandHandler<ChangeColour>, ChangeColourCommandHandler>();

builder.Services.AddHostedService<ChangeColourBackgroundService>();
builder.Services.AddHostedService<PrintColourBackgroundService>();

builder.AddCoreServices();
builder.AddEventBroadcaster();
builder.AddEventPump();
builder.AddAzureServices(Defaults.Azure.AzuriteConnectionString);

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