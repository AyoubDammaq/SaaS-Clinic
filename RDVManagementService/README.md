# 🏥 RDVManagementService - Clean Architecture

Ce projet implémente un service de gestion des rendez-vous (RDV) basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 RDVManagementService.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `RendezVousController.cs` : Gestion des opérations sur les rendez-vous.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 RDVManagementService.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `RendezVousDTO.cs` : DTO pour les données des rendez-vous.
  - `RendezVousStatDTO.cs` : DTO pour les statistiques des rendez-vous.
- `Services/` : Contient la logique métier orchestrée.
  - `RendezVousService.cs` : Service principal pour les opérations sur les rendez-vous.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
  - `IRendezVousService.cs`

### 📌 RDVManagementService.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `RendezVous.cs` : Entité représentant un rendez-vous.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
- `Enums/` : Énumérations métier.
  - `RDVStatus.cs` : Énumération pour les statuts des rendez-vous.
- `Interfaces/` : Interfaces des repositories.
  - `IRendezVousRepository.cs` : Interface pour les opérations sur les rendez-vous.

### 📌 RDVManagementService.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
  - `RendezVousDbContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `RendezVousRepository.cs` : Implémentation pour les opérations sur les rendez-vous.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `RDVManagementService.API.Controllers`
2. Le contrôleur appelle `RendezVousService` → `RDVManagementService.Application`
3. Le service utilise un repository → `RDVManagementService.Domain.Interfaces` → implémenté dans `RDVManagementService.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `RDVManagementService.API/` pour containeriser l'application :

### 📌 Prérequis

- .NET 8 SDK
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande