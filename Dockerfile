# Build stage: compile and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY JobTracker.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage: minimal ASP.NET Core image, non-root user
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# SQLite lives outside the app dir so it can be mounted as a volume
RUN mkdir -p /app/data && chown -R app:app /app/data
USER app

ENV ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/app.db;Cache=Shared"
EXPOSE 8080

ENTRYPOINT ["dotnet", "JobTracker.dll"]
