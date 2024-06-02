FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["property-price-cosmos-db/property-price-cosmos-db.csproj", "property-price-cosmos-db/"]
RUN dotnet restore "property-price-cosmos-db/property-price-cosmos-db.csproj"
COPY . .
WORKDIR "/src/property-price-cosmos-db"
RUN dotnet build "property-price-cosmos-db.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "property-price-cosmos-db.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "property-price-cosmos-db.dll"]
