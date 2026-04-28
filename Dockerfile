# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Schefco.TaskFlow.API/Schefco.TaskFlow.API.csproj Schefco.TaskFlow.API/
COPY Schefco.TaskFlow.Application/Schefco.TaskFlow.Application.csproj Schefco.TaskFlow.Application/
COPY Schefco.TaskFlow.Infrastructure/Schefco.TaskFlow.Infrastructure.csproj Schefco.TaskFlow.Infrastructure/
COPY Schefco.TaskFlow.Domain/Schefco.TaskFlow.Domain.csproj Schefco.TaskFlow.Domain/

RUN dotnet restore Schefco.TaskFlow.API/Schefco.TaskFlow.API.csproj

COPY . .

RUN dotnet publish Schefco.TaskFlow.API/Schefco.TaskFlow.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Schefco.TaskFlow.API.dll"]

