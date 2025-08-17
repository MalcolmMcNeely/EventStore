docker run --name postgres -e POSTGRES_PASSWORD=password -e POSTGRES_DB=EventStore -e POSTGRES_USER=postgres -p 5432:5432 postgres

from root:

docker build -f src/EventStore.Blazor.EFCore.Postgres/Dockerfile src