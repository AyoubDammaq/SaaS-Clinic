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
COPY ["Clinic.API/Consultation.API.csproj", "Clinic.API/"]
COPY ["Consultation.Application/Consultation.Application.csproj", "Consultation.Application/"]
COPY ["Consultation.Domain/Consultation.Domain.csproj", "Consultation.Domain/"]
COPY ["Consultation.Infrastructure/Consultation.Infrastructure.csproj", "Consultation.Infrastructure/"]
RUN dotnet restore "./Clinic.API/Consultation.API.csproj"
COPY . .
WORKDIR "/src/Clinic.API"
RUN dotnet build "./Consultation.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Consultation.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consultation.API.dll"]