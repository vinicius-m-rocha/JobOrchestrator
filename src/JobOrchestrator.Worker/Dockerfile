# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/JobOrchestrator.Domain/JobOrchestrator.Domain.csproj", "JobOrchestrator.Domain/"]
COPY ["src/JobOrchestrator.Application/JobOrchestrator.Application.csproj", "JobOrchestrator.Application/"]
COPY ["src/JobOrchestrator.Infrastructure/JobOrchestrator.Infrastructure.csproj", "JobOrchestrator.Infrastructure/"]
COPY ["src/JobOrchestrator.Worker/JobOrchestrator.Worker.csproj", "JobOrchestrator.Worker/"]
RUN dotnet restore "JobOrchestrator.Worker/JobOrchestrator.Worker.csproj"

COPY ["src/JobOrchestrator.Application/", "JobOrchestrator.Application/"]
COPY ["src/JobOrchestrator.Infrastructure/", "JobOrchestrator.Infrastructure/"]
COPY ["src/JobOrchestrator.Domain/", "JobOrchestrator.Domain/"]
COPY ["src/JobOrchestrator.Worker/", "JobOrchestrator.Worker/"]
WORKDIR /src/JobOrchestrator.Worker
RUN dotnet build "JobOrchestrator.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobOrchestrator.Worker.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobOrchestrator.Worker.dll"]