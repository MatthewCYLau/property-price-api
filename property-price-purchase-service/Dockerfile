FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["property-price-purchase-service/property-price-purchase-service.csproj", "property-price-purchase-service/"]
RUN dotnet restore "property-price-purchase-service/property-price-purchase-service.csproj"
COPY . .
WORKDIR "/src/property-price-purchase-service"
RUN dotnet build "property-price-purchase-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "property-price-purchase-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "property-price-purchase-service.dll"]
