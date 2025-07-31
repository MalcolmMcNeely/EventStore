Simple implementation of an Event Sourcing library with InMemory and Azure sample apps

To run the Azure sample run you must be running Azurite on the default ports

The breakdown of the message loops comes in 3 parts:
- `IEventTransport`: Write to the All Stream
- `IEventPump`: Keeps up to date with events from the All Stream it hasn't seen and publishes them
- `IEventBroadcaster`: Takes published messages and broadcasts them to any `IProjectionBuilder<>` which handles them

TODO List:
- [x] Also publish events to specific Projection streams
- [ ] Rebuild Projections based on their speicific Projection streams on demand
- [ ] Rebuild AggregateRoots based on their specific AggregateRoot streams on demand
- [ ] Also allow for commands on the message transport
- [ ] Allow for events to be upgraded
