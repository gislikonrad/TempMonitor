FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-bookworm-slim AS base
USER $APP_UID
WORKDIR /app
COPY ./publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 1234
ENTRYPOINT ["dotnet", "TempMonitor.dll"]
