FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-bookworm-slim AS base
RUN dotnet --list-sdks
ENV ASPNETCORE_ENVIRONMENT=Production
USER $APP_UID
WORKDIR /app
COPY ./publish .
EXPOSE 1234
ENTRYPOINT ["dotnet", "TempMonitor.dll"]
