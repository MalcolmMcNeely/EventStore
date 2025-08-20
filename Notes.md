docker run --name postgres -e POSTGRES_PASSWORD=password -e POSTGRES_DB=EventStore -e POSTGRES_USER=postgres -p 5432:5432 postgres

from root:

docker build -f src/EventStore.Blazor.EFCore.Postgres/Dockerfile src

TODO List:
- [x] Also publish events to specific Projection streams
- [x] Rebuild Projections based on their speicific Projection streams on demand
- [ ] Rebuild AggregateRoots based on their specific AggregateRoot streams on demand
- [x] Key building for projections
- [ ] Also allow for commands on the message transport ?
- [ ] Allow for events to be upgraded
- [x] EF Core implementation
- [x] DB Pooling
- [ ] Use EF interceptors to write to the all stream ?
- [x] Write some tests using [Verify](https://github.com/VerifyTests/Verify)
- [ ] Code generation to help out JsonSerializable ?
- [x] [Test Containers](https://testcontainers.com/)
- [ ] TUnit (when rider can discover tests)