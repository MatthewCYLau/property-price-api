FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["property-price-kafka-service/property-price-kafka-service.csproj", "property-price-kafka-service/"]
RUN dotnet restore "property-price-kafka-service/property-price-kafka-service.csproj"
COPY . .
WORKDIR "/src/property-price-kafka-service"
RUN dotnet build "property-price-kafka-service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "property-price-kafka-service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "property-price-kafka-service.dll"]
