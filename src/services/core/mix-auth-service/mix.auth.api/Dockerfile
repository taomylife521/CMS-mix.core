#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["platform/core/mix-heart/nuget.config", "platform/core/mix-heart/"]
COPY ["services/core/mix-auth-service/mix.auth.api/mix.auth.api.csproj", "services/core/mix-auth-service/mix.auth.api/"]
COPY ["applications/mixcore.host.aspire.ServiceDefaults/mixcore.host.aspire.ServiceDefaults.csproj", "applications/mixcore.host.aspire.ServiceDefaults/"]
COPY ["modules/mix.grpc/mix.grpc.csproj", "modules/mix.grpc/"]
COPY ["platform/mix.library/mix.library.csproj", "platform/mix.library/"]
COPY ["platform/core/mix.mixdb.event/mix.mixdb.event.csproj", "platform/core/mix.mixdb.event/"]
COPY ["platform/mix.mixdb/mix.mixdb.csproj", "platform/mix.mixdb/"]
COPY ["platform/mix.database/mix.database.csproj", "platform/mix.database/"]
COPY ["platform/mix.shared/mix.shared.csproj", "platform/mix.shared/"]
COPY ["platform/core/mix-heart/src/mix.heart/mix.heart.csproj", "platform/core/mix-heart/src/mix.heart/"]
COPY ["platform/mix.constant/mix.constant.csproj", "platform/mix.constant/"]
COPY ["platform/mix.service/mix.service.csproj", "platform/mix.service/"]
COPY ["platform/mix.identity/mix.identity.csproj", "platform/mix.identity/"]
COPY ["platform/mix.auth/mix.auth.csproj", "platform/mix.auth/"]
COPY ["platform/mix.quartz/mix.quartz.csproj", "platform/mix.quartz/"]
COPY ["platform/mix.queue/mix.queue.csproj", "platform/mix.queue/"]
COPY ["services/core/mix-message-queue/mix.mq.server/mix.mq.server.csproj", "services/core/mix-message-queue/mix.mq/"]
COPY ["services/core/mix-message-queue/mix.mq.lib/mix.mq.lib.csproj", "services/core/mix-message-queue/mix.mq.lib/"]
COPY ["platform/mix.signalr/mix.signalr.csproj", "platform/mix.signalr/"]
COPY ["platform/mix.repodb/mix.repodb.csproj", "platform/mix.repodb/"]
COPY ["platform/mix.communicator/mix.communicator.csproj", "platform/mix.communicator/"]
COPY ["platform/mix.signalr.hub/mix.signalr.hub.csproj", "platform/mix.signalr.hub/"]
COPY ["platform/mix.log/mix.log.lib.csproj", "platform/mix.log/"]
RUN dotnet restore "./services/core/mix-auth-service/mix.auth.api/mix.auth.api.csproj"
COPY . .
WORKDIR "/src/services/core/mix-auth-service/mix.auth.api"
RUN dotnet build "./mix.auth.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./mix.auth.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mix.auth.api.dll"]