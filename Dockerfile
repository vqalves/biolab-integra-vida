# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY BiopasIntegraVida.sln ./BiopasIntegraVida.sln
COPY src/BiopasIntegraVida.Domain/BiopasIntegraVida.Domain.csproj ./src/BiopasIntegraVida.Domain/BiopasIntegraVida.Domain.csproj
COPY src/BiopasIntegraVida.Infrastructure/BiopasIntegraVida.Infrastructure.csproj ./src/BiopasIntegraVida.Infrastructure/BiopasIntegraVida.Infrastructure.csproj
COPY src/BiopasIntegraVida.InterPlayers/BiopasIntegraVida.InterPlayers.csproj ./src/BiopasIntegraVida.InterPlayers/BiopasIntegraVida.InterPlayers.csproj
COPY src/BiopasIntegraVida.Web/BiopasIntegraVida.Web.csproj ./src/BiopasIntegraVida.Web/BiopasIntegraVida.Web.csproj
COPY tests/BiopasIntegraVida.IntegrationTests/BiopasIntegraVida.IntegrationTests.csproj ./tests/BiopasIntegraVida.IntegrationTests/BiopasIntegraVida.IntegrationTests.csproj
COPY tests/BiopasIntegraVida.UnitTests/BiopasIntegraVida.UnitTests.csproj ./tests/BiopasIntegraVida.UnitTests/BiopasIntegraVida.UnitTests.csproj
# RUN echo $(ls -lR -1 .)
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
WORKDIR /source/src/BiopasIntegraVida.Web
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "BiopasIntegraVida.Web.dll"]