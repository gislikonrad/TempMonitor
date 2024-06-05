FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-bookworm-slim AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.203-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TempMonitor/TempMonitor.csproj", "TempMonitor/"]
RUN dotnet restore "TempMonitor/TempMonitor.csproj"
COPY . .
WORKDIR "/src/TempMonitor"
RUN dotnet build "TempMonitor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TempMonitor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 1234
ENTRYPOINT ["dotnet", "TempMonitor.dll"]
