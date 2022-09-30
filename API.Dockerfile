# Build stage
# 'core' has been dropped from the repo name. See https://github.com/dotnet/dotnet-docker/issues/1939
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY *.sln .
COPY ["UTB.API/*.csproj", "./UTB.API/"]
COPY ["UTB.Contracts/*.csproj", "./UTB.Contracts/"]
COPY ["UTB.Data/*.csproj", "./UTB.Data/"]
COPY ["UTB.Jobs/*.csproj", "./UTB.Jobs/"]

RUN ["dotnet", "restore", "./UTB.API/UTB.API.csproj"]

COPY ["UTB.API/", "./UTB.API/"]
COPY ["UTB.Contracts/", "./UTB.Contracts/"]
COPY ["UTB.Data/", "./UTB.Data/"]
COPY ["UTB.Jobs/", "./UTB.Jobs/"]

RUN ["dotnet", "publish", "./UTB.API/UTB.API.csproj", "-c", "Release", "-o", "out"]

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UTB.API.dll"]
