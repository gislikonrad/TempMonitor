FROM mcr.microsoft.com/dotnet/aspnet:8.0.6-bookworm-slim
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
COPY ./publish .
EXPOSE 1234
ENTRYPOINT ["/usr/share/dotnet/dotnet", "TempMonitor.dll"]
