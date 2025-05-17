FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Jobs/ExportPro.Export.Job.ServiceHost/ ./ExportPro.Jobs/ExportPro.Export.Job.ServiceHost/
RUN dotnet restore ./ExportPro.Jobs/ExportPro.Export.Job.ServiceHost/ExportPro.Export.Job.ServiceHost.csproj
WORKDIR /src/ExportPro.Jobs/ExportPro.Export.Job.ServiceHost
RUN dotnet publish "ExportPro.Export.Job.ServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.Export.Job.ServiceHost.dll"]