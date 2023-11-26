FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY ["property-price-api/property-price-api.csproj", "property-price-api/"]
COPY ["property-price-ingest/property-price-ingest.csproj", "property-price-ingest/"]

RUN dotnet restore "property-price-ingest/property-price-ingest.csproj"
COPY . .
WORKDIR /app/property-price-ingest
RUN dotnet build "property-price-ingest.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "property-price-ingest.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "property-price-ingest.dll"]