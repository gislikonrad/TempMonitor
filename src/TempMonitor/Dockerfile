FROM mcr.microsoft.com/dotnet/aspnet:8.0.6-bookworm-slim
ENV ASPNETCORE_ENVIRONMENT=Production
USER $APP_UID
WORKDIR /app
COPY ./publish .
EXPOSE 1234
ENTRYPOINT ["/usr/share/dotnet/dotnet", "TempMonitor.dll"]
