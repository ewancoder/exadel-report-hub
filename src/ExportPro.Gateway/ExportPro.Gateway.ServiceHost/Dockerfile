#to build this dockerfile from /src  docker build -f ExportPro.Gateway/ExportPro.Gateway/Dockerfile . -t giorgaaa/exportproocelot
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Gateway/ ./ExportPro.Gateway/
RUN dotnet restore ExportPro.Gateway/ExportPro.Gateway.ServiceHost/ExportPro.Gateway.ServiceHost.csproj

WORKDIR /src/ExportPro.Gateway/ExportPro.Gateway.ServiceHost/
RUN dotnet build -c Release --no-restore
RUN dotnet publish "ExportPro.Gateway.ServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.Gateway.ServiceHost.dll"]
