# 🏥 ConsultationManagementService - Clean Architecture

Ce projet implémente un service de gestion des consultations basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet


### 📌 ConsultationManagementService.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `ConsultationController.cs` : Gestion des opérations sur les consultations.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 ConsultationManagementService.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `ConsultationDTO.cs` : DTO pour les données des consultations.
  - `DocumentMedicalDTO.cs` : DTO pour les documents médicaux associés.
- `Services/` : Contient la logique métier orchestrée.
  - `ConsultationService.cs` : Service principal pour les opérations sur les consultations.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
  - `IConsultationService.cs`

### 📌 ConsultationManagementService.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `Consultation.cs` : Entité représentant une consultation.
  - `DocumentMedical.cs` : Entité pour les documents médicaux.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
- `Enums/` : Énumérations métier.
- `Interfaces/` : Interfaces des repositories.
  - `IConsultationRepository.cs` : Interface pour les opérations sur les consultations.

### 📌 ConsultationManagementService.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
  - `ConsultationDbContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `ConsultationRepository.cs` : Implémentation pour les opérations sur les consultations.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `ConsultationManagementService.API.Controllers`
2. Le contrôleur appelle `ConsultationService` → `ConsultationManagementService.Application`
3. Le service utilise un repository → `ConsultationManagementService.Domain.Interfaces` → implémenté dans `ConsultationManagementService.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `ConsultationManagementService.API/` pour containeriser l'application :


### 📌 Prérequis

- .NET 8 SDK
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande