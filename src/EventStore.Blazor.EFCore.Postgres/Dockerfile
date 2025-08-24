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

# Install Auto Insturmentation
ARG OTEL_VERSION=1.7.0
RUN apt-get update && apt-get install -y unzip curl
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
RUN apt-get update && apt-get install -y unzip && \
    OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto" sh otel-dotnet-auto-install.sh
RUN chmod +x /otel-dotnet-auto/instrument.sh

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /
COPY --from=build /otel-dotnet-auto ./otel-dotnet-auto
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
#ENTRYPOINT ["dotnet", "EventStore.Blazor.EFCore.Postgres.dll"]
ENTRYPOINT ["/bin/bash", "-c", "source /otel-dotnet-auto/instrument.sh && dotnet EventStore.Blazor.EFCore.Postgres.dll"]
