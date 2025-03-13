# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY BiolabIntegraVida.sln ./BiolabIntegraVida.sln
COPY src/BiolabIntegraVida.Domain/BiolabIntegraVida.Domain.csproj ./src/BiolabIntegraVida.Domain/BiolabIntegraVida.Domain.csproj
COPY src/BiolabIntegraVida.Infrastructure/BiolabIntegraVida.Infrastructure.csproj ./src/BiolabIntegraVida.Infrastructure/BiolabIntegraVida.Infrastructure.csproj
COPY src/BiolabIntegraVida.InterPlayers/BiolabIntegraVida.InterPlayers.csproj ./src/BiolabIntegraVida.InterPlayers/BiolabIntegraVida.InterPlayers.csproj
COPY src/BiolabIntegraVida.Web/BiolabIntegraVida.Web.csproj ./src/BiolabIntegraVida.Web/BiolabIntegraVida.Web.csproj
COPY tests/BiolabIntegraVida.IntegrationTests/BiolabIntegraVida.IntegrationTests.csproj ./tests/BiolabIntegraVida.IntegrationTests/BiolabIntegraVida.IntegrationTests.csproj
COPY tests/BiolabIntegraVida.UnitTests/BiolabIntegraVida.UnitTests.csproj ./tests/BiolabIntegraVida.UnitTests/BiolabIntegraVida.UnitTests.csproj
# RUN echo $(ls -lR -1 .)
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
WORKDIR /source/src/BiolabIntegraVida.Web
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "BiolabIntegraVida.Web.dll"]