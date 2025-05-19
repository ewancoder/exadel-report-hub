#to build this dockerfile from /src  do docker build -f ExportPro.Front/Dockerfile . -t giorgaaa/exportprofront
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ExportPro.Front/ ./ExportPro.Front/
RUN dotnet restore ExportPro.Front/ExportPro.Front.csproj
WORKDIR /src/ExportPro.Front/
RUN dotnet publish "ExportPro.Front.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExportPro.Front.dll"]