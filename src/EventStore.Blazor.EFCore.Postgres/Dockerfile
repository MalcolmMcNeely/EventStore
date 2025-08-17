FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EventStore.Blazor.EFCore.Postgres/*.csproj", "EventStore.Blazor.EFCore.Postgres/"]
COPY ["EventStore.SampleApp.Domain/*.csproj", "EventStore.SampleApp.Domain/"]
COPY ["EventStore.Core/*.csproj", "EventStore.Core/"]
COPY ["EventStore.EFCore.Postgres/*.csproj", "EventStore.EFCore.Postgres/"]
RUN dotnet restore "EventStore.Blazor.EFCore.Postgres/EventStore.Blazor.EFCore.Postgres.csproj"
COPY . .
RUN dotnet publish "EventStore.Blazor.EFCore.Postgres//EventStore.Blazor.EFCore.Postgres.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventStore.Blazor.EFCore.Postgres.dll"]
