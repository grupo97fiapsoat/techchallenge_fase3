# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/FastFood.Api/FastFood.Api.csproj", "src/FastFood.Api/"]
COPY ["src/FastFood.Application/FastFood.Application.csproj", "src/FastFood.Application/"]
COPY ["src/FastFood.Domain/FastFood.Domain.csproj", "src/FastFood.Domain/"]
COPY ["src/FastFood.Infrastructure/FastFood.Infrastructure.csproj", "src/FastFood.Infrastructure/"]

RUN dotnet restore "src/FastFood.Api/FastFood.Api.csproj"

# Copy source code
COPY src/ src/

# Build the application
WORKDIR "/src/src/FastFood.Api"
RUN dotnet build "FastFood.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FastFood.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --ingroup dotnet dotnet

# Create directory for certificates
RUN mkdir -p /app/certs && chown -R dotnet:dotnet /app/certs

# Copy published app
COPY --from=publish /app/publish .

# Change ownership to dotnet user
RUN chown -R dotnet:dotnet /app
USER dotnet

# Configure ports
EXPOSE 8080 8443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f -k https://localhost:8443/health || curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FastFood.Api.dll"]
