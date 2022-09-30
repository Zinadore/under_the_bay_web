# Build stage
# 'core' has been dropped from the repo name. See https://github.com/dotnet/dotnet-docker/issues/1939
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY *.sln .
COPY ["UTB.Console/*.csproj", "./UTB.Console/"]
COPY ["UTB.Contracts/*.csproj", "./UTB.Contracts/"]
COPY ["UTB.Data/*.csproj", "./UTB.Data/"]
COPY ["UTB.Jobs/*.csproj", "./UTB.Jobs/"]

RUN ["dotnet", "restore", "./UTB.Console/UTB.Console.csproj"]

COPY ["UTB.Console/", "./UTB.Console/"]
COPY ["UTB.Contracts/", "./UTB.Contracts/"]
COPY ["UTB.Data/", "./UTB.Data/"]
COPY ["UTB.Jobs/", "./UTB.Jobs/"]

RUN ["dotnet", "publish", "./UTB.Console/UTB.Console.csproj", "-c", "Release", "-o", "out"]

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY ./wait-for-it.sh .
RUN chmod +x ./wait-for-it.sh
# ENTRYPOINT ["dotnet", "UTB.Console.dll"]
ENTRYPOINT ./wait-for-it.sh -s -h $UTB_PG_HOST -p $UTB_PG_PORT -t 60 && dotnet UTB.Console.dll