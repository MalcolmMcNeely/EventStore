Implementation of an Event Sourcing library with InMemory, Azure, and Blazor/EF Core sample apps

Technologies used:

- Azure storage
- Entity framework core
- Blazor
- Open telemetry auto instrumentation
- Grafana
- Grafana Loki
- Prometheus
- Jaeger
- Docker
- NUnit
- Test containers
- Verify

# Running Blazor.EFCore.Postgres

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/)

## Running

```ps1
cd .\src\
docker compose up --build
```

http://localhost:5256/

# Running SampleApp.Azure

## Prerequisites

- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage)

```ps1
npm install -g azurite
```

## Running

F5

# Architecture

The breakdown of the message loops comes in 3 parts:

- `IEventTransport`: Writes to the All Stream
- `IEventPump`: Keeps up to date with events from the All Stream it hasn't seen and publishes them
- `IEventBroadcaster`: Takes published messages and broadcasts them to any `IProjectionBuilder<>` which handles them

Both `CommandHandlers` and `ProjectionBuilders` are designed to be self-contained pieces of code making it ideal for
vertical slices/screaming architecture patterns. When it comes to scaling out your application you will have discrete
units which you can move out into an Azure Function or AWS Lambda. See EventStore.SampleApp.Domain for an example of
this.

When an event is dispatched via a `CommandHandler`, which must be done via a `UnitOfWork`, it will write both to the All
Stream but also its own aggregate root stream to allow inspection of any given aggregate root but to also facilitate
on-demand rebuilding. Similarly, when an event is dispatched to a projection builder via the `IEventBroadcaster`, it
will write an event to a projection specific stream.

Current features:

- [x] Projection rebuilding: combined with event upgrading allows fast on demand migrations of your application data
  structure

Future features:

- [ ] Event upgrades: allow migrations of the application data structure
- [ ] Aggregate root rebuilding: combined with event upgrading allows fast on demand migrations of your application data
  structure

