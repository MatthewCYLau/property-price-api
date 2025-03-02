FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["property-price-kafka-service/property-price-kafka-service.csproj", "property-price-kafka-service/"]
COPY ["property-price-kafka-consumer/property-price-kafka-consumer.csproj", "property-price-kafka-consumer/"]
RUN dotnet restore "property-price-kafka-consumer/property-price-kafka-consumer.csproj"
COPY . .
WORKDIR "/src/property-price-kafka-consumer"
RUN dotnet build "property-price-kafka-consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "property-price-kafka-consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "property-price-kafka-consumer.dll"]
