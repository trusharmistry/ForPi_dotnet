FROM mcr.microsoft.com/dotnet/core/sdk:6.0 AS build
WORKDIR /usr/src/app

# copy csproj and restore as distinct layers
COPY Pi.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /usr/src/app
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:6.0 AS runtime
WORKDIR /usr/src/app/
COPY --from=build /usr/src/app/out ./

ENTRYPOINT ["dotnet", "Pi.dll"]

