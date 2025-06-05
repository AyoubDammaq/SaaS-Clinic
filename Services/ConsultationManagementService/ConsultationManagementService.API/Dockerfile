# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8091
EXPOSE 8092


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConsultationManagementService.API/ConsultationManagementService.API.csproj", "ConsultationManagementService.API/"]
COPY ["ConsultationManagementService.Application/ConsultationManagementService.Application.csproj", "ConsultationManagementService.Application/"]
COPY ["ConsultationManagementService.Domain/ConsultationManagementService.Domain.csproj", "ConsultationManagementService.Domain/"]
COPY ["ConsultationManagementService.Infrastructure/ConsultationManagementService.Infrastructure.csproj", "ConsultationManagementService.Infrastructure/"]
RUN dotnet restore "./ConsultationManagementService.API/ConsultationManagementService.API.csproj"
COPY . .
WORKDIR "/src/ConsultationManagementService.API"
RUN dotnet build "./ConsultationManagementService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConsultationManagementService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsultationManagementService.API.dll"]