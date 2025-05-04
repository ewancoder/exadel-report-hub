#to build this dockerfile from /src  docker build -f ExportPro.Export/Dockerfile . -t giorgaaa/exportpro.export
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Common/ExportPro.Common.Shared/Settings/serilogsettings.json ./ExportPro.Common/
COPY ExportPro.Export/ ./ExportPro.Export/
RUN dotnet restore ExportPro.Export/ExportPro.Export.sln
WORKDIR /src/ExportPro.Export/ExportPro.Export.ServiceHost
RUN dotnet publish "ExportPro.Export.ServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /src/ExportPro.Common/serilogsettings.json ./Settings/serilogsettings.json
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.Export.ServiceHost.dll"]