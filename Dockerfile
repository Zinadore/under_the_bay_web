# Build stage
# 'core' has been dropped from the repo name. See https://github.com/dotnet/dotnet-docker/issues/1939
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /app
COPY *.sln .
COPY ["Under the Bay.API/*.csproj", "./Under the Bay.API/"]
COPY ["Under the Bay.Data/*.csproj", "./Under the Bay.Data/"]
COPY ["DataFetcher/*.csproj", "./DataFetcher/"]
RUN dotnet restore

COPY ["Under the Bay.API/", "./Under the Bay.API/"]
COPY ["Under the Bay.Data/", "./Under the Bay.Data/"]
COPY ["DataFetcher/", "./DataFetcher/"]

RUN ["dotnet", "publish", "./Under the Bay.API/Under the Bay.API.csproj", "-c", "Release", "-o", "out"]

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Under the Bay.API.dll"]
