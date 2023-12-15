# Property Price API

An ASP.NET Core web API which returns suggested property offer price

![cicd cloud run workflow](https://github.com/MatthewCYLau/property-price-api/actions/workflows/cicd-cloud-run.yaml/badge.svg)

API URL here: [`https://property-price-api-3i2mtbjusq-ew.a.run.app`](https://property-price-api-3i2mtbjusq-ew.a.run.app)

The list of repositories are as follow:

- ASP.NET Core web API repository [here](https://github.com/MatthewCYLau/property-price-api)
- React with Vite client repository [here](https://github.com/MatthewCYLau/property-price-client)
- Google Cloud Platform (GCP) with Terraform infrastructure repository [here](https://github.com/MatthewCYLau/property-price-infrastructure)

## Pre-requisite

Ensure you have installed [.NET SDK](https://dotnet.microsoft.com/en-us/download):

```bash
dotnet --version
```

## Run app locally

```bash
dotnet run
curl http://localhost:5049/ping
```

## Build/run app locally via dotnet

```bash
dotnet restore
dotnet publish -c Release -o out
cd out
dotnet property-price-api.dll --environment=Development
```

## Build/run app via Docker

```bash
cd property-price-api
docker build -t property-price-api:v1 .
docker run -p 5001:8080 property-price-api:v1
curl http://localhost:5001/ping
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
