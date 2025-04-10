#to build this dockerfile from /src  docker build -f ExportPro.StorageService/Dockerfile .  
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Common/ ./ExportPro.Common/
COPY ExportPro.StorageService/ ./ExportPro.StorageService/
RUN dotnet restore ExportPro.StorageService/ExportPro.StorageService.sln
WORKDIR /src/ExportPro.StorageService/ExportPro.StorageService.API
RUN dotnet publish "ExportPro.StorageService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.StorageService.API.dll"]