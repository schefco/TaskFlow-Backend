# STEP 1 — Build your .NET app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything into the container
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish the app in Release mode
RUN dotnet publish -c Release -o /app/publish

# STEP 2 — Run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files from the build step
COPY --from=build /app/publish .

# Render requires port 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start your API
ENTRYPOINT ["dotnet", "Schefco.TaskFlow.API.dll"]
