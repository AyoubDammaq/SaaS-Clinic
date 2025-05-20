# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8095
EXPOSE 8096


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Reporting.API/Reporting.API.csproj", "Reporting.API/"]
COPY ["Reporting.Application/Reporting.Application.csproj", "Reporting.Application/"]
COPY ["Reporting.Domain/Reporting.Domain.csproj", "Reporting.Domain/"]
COPY ["Reporting.Infrastructure/Reporting.Infrastructure.csproj", "Reporting.Infrastructure/"]
RUN dotnet restore "./Reporting.API/Reporting.API.csproj"
COPY . .
WORKDIR "/src/Reporting.API"
RUN dotnet build "./Reporting.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reporting.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reporting.API.dll"]