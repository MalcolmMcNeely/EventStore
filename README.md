Implementation of an Event Sourcing library with InMemory, Azure, and EF Core sample apps

To run the Azure sample run you must be running Azurite on the default ports

EF Core sample app has been developed against a Postgres database

The breakdown of the message loops comes in 3 parts:
- `IEventTransport`: Writes to the All Stream
- `IEventPump`: Keeps up to date with events from the All Stream it hasn't seen and publishes them
- `IEventBroadcaster`: Takes published messages and broadcasts them to any `IProjectionBuilder<>` which handles them

TODO List:
- [x] Also publish events to specific Projection streams
- [x] Rebuild Projections based on their speicific Projection streams on demand
- [ ] Rebuild AggregateRoots based on their specific AggregateRoot streams on demand
- [ ] Key building for projections
- [ ] Also allow for commands on the message transport
- [ ] Allow for events to be upgraded
- [x] EF Core implementation
- [ ] Sort out config/defaults
- [ ] DB context pooling
