FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/FastFood.Api/FastFood.Api.csproj", "FastFood.Api/"]
COPY ["src/FastFood.Application/FastFood.Application.csproj", "FastFood.Application/"]
COPY ["src/FastFood.Domain/FastFood.Domain.csproj", "FastFood.Domain/"]
COPY ["src/FastFood.Infrastructure/FastFood.Infrastructure.csproj", "FastFood.Infrastructure/"]
RUN dotnet restore "FastFood.Api/FastFood.Api.csproj"
COPY . .
WORKDIR "/src/FastFood.Api"
RUN dotnet build "FastFood.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FastFood.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FastFood.Api.dll"]
