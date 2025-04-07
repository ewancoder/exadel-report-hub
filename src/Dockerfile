FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 5001
EXPOSE 7067
EXPOSE 5044
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore ExportPro.sln
WORKDIR "/src/ExportPro.Auth/ExportPro.Auth.ServiceHost"
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "ExportPro.Auth.ServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExportPro.Auth.ServiceHost.dll"]