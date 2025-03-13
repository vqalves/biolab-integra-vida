# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY MerckCuida.sln ./MerckCuida.sln
COPY src/MerckCuida.Domain/MerckCuida.Domain.csproj ./src/MerckCuida.Domain/MerckCuida.Domain.csproj
COPY src/MerckCuida.Infrastructure/MerckCuida.Infrastructure.csproj ./src/MerckCuida.Infrastructure/MerckCuida.Infrastructure.csproj
COPY src/MerckCuida.InterPlayers/MerckCuida.InterPlayers.csproj ./src/MerckCuida.InterPlayers/MerckCuida.InterPlayers.csproj
COPY src/MerckCuida.Web/MerckCuida.Web.csproj ./src/MerckCuida.Web/MerckCuida.Web.csproj
COPY tests/MerckCuida.IntegrationTests/MerckCuida.IntegrationTests.csproj ./tests/MerckCuida.IntegrationTests/MerckCuida.IntegrationTests.csproj
COPY tests/MerckCuida.UnitTests/MerckCuida.UnitTests.csproj ./tests/MerckCuida.UnitTests/MerckCuida.UnitTests.csproj
# RUN echo $(ls -lR -1 .)
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
WORKDIR /source/src/MerckCuida.Web
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MerckCuida.Web.dll"]