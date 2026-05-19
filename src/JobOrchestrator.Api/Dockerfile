FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/JobOrchestrator.Domain/JobOrchestrator.Domain.csproj", "JobOrchestrator.Domain/"]
COPY ["src/JobOrchestrator.Application/JobOrchestrator.Application.csproj", "JobOrchestrator.Application/"]
COPY ["src/JobOrchestrator.Infrastructure/JobOrchestrator.Infrastructure.csproj", "JobOrchestrator.Infrastructure/"]
COPY ["src/JobOrchestrator.Api/JobOrchestrator.Api.csproj", "JobOrchestrator.Api/"]
RUN dotnet restore "JobOrchestrator.Api/JobOrchestrator.Api.csproj"


COPY ["src/JobOrchestrator.Domain/", "JobOrchestrator.Domain/"]
COPY ["src/JobOrchestrator.Application/", "JobOrchestrator.Application/"]
COPY ["src/JobOrchestrator.Infrastructure/", "JobOrchestrator.Infrastructure/"]
COPY ["src/JobOrchestrator.Api/", "JobOrchestrator.Api/"]
WORKDIR /src/JobOrchestrator.Api
RUN dotnet build "JobOrchestrator.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobOrchestrator.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobOrchestrator.Api.dll"]