#to build this dockerfile from /src  docker build -f ExportPro.Auth/Dockerfile .  -t giorgaaa/exportpro.authservice
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Common/ ./ExportPro.Common/
COPY ExportPro.Auth/ ./ExportPro.Auth/
RUN dotnet restore ExportPro.Auth/ExportPro.Auth.sln
WORKDIR /src/ExportPro.Auth/ExportPro.Auth.ServiceHost
RUN dotnet publish "ExportPro.Auth.ServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.Auth.ServiceHost.dll"]